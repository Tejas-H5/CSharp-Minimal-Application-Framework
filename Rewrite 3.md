NOTE: I am currently rewriting this a third time, so it isn't really completed.

# MinimalAF rewrite 3

What I want to get done:
- Simplify the hell out of this framework. it isn't exactly minimal at the moment
	1. Remove the child system
	3. No render/update method, just one method that does both. Full immediate mode aesthetic code (will trigger some people. If you wanted performance, use C or C++ or Rust. You're here because you want to write this stuff quickly and easily but HTML/javascript isn't a good option)
	4. Rewrite windowing with GLFW bindings directly, so we can have more than 1 window.


Splitting the render and the update only makes sense for games, and not really for UI API.

The only reason why I would ever want to split the 'update loop' from the 'render loop' is to poll for events more frequently. This is beneficial in games where precise input detection is required. The best (and only) case I am familiar with (but there are probably a lot more) is that of osu!. Players would crank up their osu FPS to the 1000s, in order to get smoother inputs in the game. Because the render loop was tightly coupled to the update loop, this was a performance L, which is a shame because if they were decoupled somehow, even a normal computer should be able to update thousands of times per second. But other than that, there is no real use-case for splitting out the render and update loops. 

I am thinking of rewriting the framework. But, the render function gets called multiple times, in 'render' mode, or in 'update' mode. 
The code that performs rendering and updating looks identical.
But there is some backend static flag such that in 'render' mode, all rendering functions are no-ops, and in 'update' mode, all updating functions are no-ops.
I'm not sure how good an idea this is, but I really like looking at the immediate mode UI code, and if the only reason why I didn't make my API immediate mode already was because of this problem, that would be a shame.

This is what some basic UI code would look like at the moment:
``` C#
// Before:
class FilePanel : Element {
	void Render(RenderContext ctx) {
		// draw background
		SetFillColor(Colors.Black);
		SetOutlineColor(Colors.White)
		DrawRect(ctx.Rect);

		// list files
		// (Keep this code up to date with the update loop)
		SetFont("source code PRO", 12);
		var x = r.X1 + 10;
		var y = r.Y1;
		SetFillColor(Colors.White);
		for (var file of files) {
			// 0 height rectangle. We don't know how tall the button will be
			var buttonRect = new Rect(x, y, x + ctx.width, y);
			// now we know the size of the rect
			buttonRect = Button.Render(ctx.WithRect(buttonRect), file.Name); 

			y -= buttonRect.Height;

			if (y < r.Y0) break;
		}
	}

	void Update(RenderContext ctx) {
		// list files
		// (Keep this code up to date with the render loop)
		SetFont("source code PRO", 12);
		var x = r.X1 + 10;
		var y = r.Y1;
		SetFillColor(Colors.White);
		for (var file of files) {
			// 0 height rectangle. We don't know how tall the button will be
			var buttonRect = new Rect(x, y, x + ctx.width, y);
			// now we know the size of the rect
			buttonRect = Button.GetButtonRect(ctx.WithRect(buttonRect), file.Name); 

			if (Input.GetMouse(buttonRect)) {
				// Do something with this file. 
			}

			y -= buttonRect.Height;

			if (y < r.Y0) break;
		}
	}
}
```

After, it would look like this:

```C#

// ---- After:


class FilePanel : Element {
	public float scrollY; 
	public File[] files;

	void Render(RenderContext ctx) {
		scrollY += ctx.HandleScrollY(ctx.Rect);

		// draw background
		SetFillColor(Colors.Black);
		SetOutlineColor(Colors.White)
		DrawRect(ctx.Rect);

		// list files

		SetFont("source code PRO", 12);
		var x = r.X1 + 10;
		var y = r.Y1;
		SetFillColor(Colors.White);
		for (var file of files) {

			// 0 height rectangle. We don't know how tall the button will be
			var buttonRect = new Rect(x, y, x + ctx.width, y);
			// now we know the size of the rect
			buttonRect = Button.Render(ctx.WithRect(buttonRect), file.Name); 

			if (Input.GetMouse(buttonRect)) {
				// Do something with this file. 
			}

			y -= buttonRect.Height;

			if (y < r.Y0) break;
		}
	}
}
```

But if we are removing elements, we would need to keep track of the current screen rectangle somehow. 
This can now be done using some RenderContext struct that gets passed down via the Render function. 
And it is a struct, so that it can be passed via value and not reference, which is important for this kind of thing.


Another thing that these decisions allow is for me to remove the `OnLayout` method, which was being called to position elements on resizes. I think there was some two way binding going on there, where  a child resizing would propagate a resize event up the chain, or something. Anyway, not sure what is going on there, but it is not clear where elements are being rendered when I look at the render method. Now, the layout would be determined in the Render method as well.


Maybe the part that I didn't consider was the bit about maintaining a coordinate system that is relative to the current rectangle.
Before, I was setting a view and projection matrix behind the scenes for each rectangle to keep all of the drawing commands relative to the rect that they are in. 
This won't work now, if I am simply passing down a context like RenderingContext:

``` C#
class App : IRenderable {
	Subcomponent c;
	App() { ... }

	void Render(RenderContext ctx) {
		...
		c.Render(
			ctx.Width(ctx.VW * 0.5).Offset(10)
		);
	}
}

class Subcomponent : IRenderable {
	void Render(RenderContext ctx) {
		// OpenGL matrices have not been set. Coordinates have not been set.
		// All Drawing ops will still be relative to the parent. Now what?
	}
}

```

I can't think of a good solution to this. Here are two conventions you could use, but nothing in the framework will enforce this or help debug issues with this:

``` C#
class App : IRenderable {
	... 
	void Render(RenderContext ctx) {
		...
		c.Render(
			ctx.Width(ctx.VW * 0.5).Offset(10).Use()
		);
	}
}
```

Or, you would do something this:

``` C#
class App : IRenderable {
	... 
	void Render(RenderContext ctx) {
		...
		c.Render(
			ctx.Width(ctx.VW * 0.5).Offset(10)
		);
	}
}

class Subcomponent : IRenderable {
	void Render(RenderContext ctx) {
		ctx.Use();

		// business as usual
	}
}

```

One idea is to use some sort of weird code-aesthetic builder pattern:

``` C#
struct RenderContext {
	...
	RenderContextBuilder Offset() { ... }
}

struct RenderContextBuilder {
	RenderContextBuilder Offset() { ... }
	...

	RenderContext Use() {
		// activate this new render context, and return it
	}
}

...


class App : IRenderable {
	... 
	void Render(RenderContext ctx) {
		...
		c.Render(
			ctx.New()
				.Width(ctx.VW * 0.5)
				.Offset(10)
				.Use()
		);
	}
}
```

This would enforce that people call Use() before passing in any render contexts, to remove the incorrect type compile error.

But what if you wanted to render something, and then render something over top?

``` C#
class App : IRenderable {
	... 
	void Render(RenderContext ctx) {
		...
		c.Render(
			ctx.New()
				.Width(ctx.VW * 0.5)
				.Offset(10)
				.Use()
		);

		// This would be required to make the next code work as expected:
		// ctx.Use();

		if (model.IsInvalidStateOrSomething) {
			// Now render a red rectangle over top
			ctx.SetDrawColor(AppColors.Invalid, 0.5f);
			ctx.DrawRect(ctx.Rect);	// Won't work, because we never switched back to our render context.
		}
	}
}
```

The javascript way might be something like this:
``` C#
	void Render(RenderContext ctx) {
		// RenderChild handles re-activating ctx
		Framework.RenderChild(ctx, () => {
			c.Render(ctx.Width(ctx.VW * 0.5)
				.Offset(10)
				.Use()
			);
		})

		... other drawing code
	}

```

but most of the time it is unnecessary to re-activate our coordinate system, so we are just wasting cpu cycles.

Think I will just stick with the manual way using convention for now.




## Garbage collector pauses

I have found that this is happening quite a lot. Probably because of the string allocations.
I will attempt to rewrite this framework to minimize the ocurrances of these (but idk what is causing them rn, will keep you posted here ...)