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
	File[] files;

	void UseButtonFont() {
		SetFont("source code PRO", 12);
		SetFillColor(Colors.White);
	}

	public void SetFiles(File[] files) {
		files = newFiles;
	}

	public override void Render() {
		// draw background
		SetFillColor(Colors.Black);
		SetOutlineColor(Colors.White)
		DrawRect(ctx.Rect);

		// list files
		// (Keep this code up to date with the update loop)
		UseButtonFont();
		var x = r.X1 + 10;
		var y = r.Y1;
		foreach (var file of files) {
			// 0 height rectangle. We don't know how tall the button will be
			var buttonRect = new Rect(x, y, x + ctx.width, y);
			// now we know the size of the rect
			buttonRect = Button.Render(ctx.WithRect(buttonRect), file.Name); 

			y -= buttonRect.Height;

			if (y < r.Y0) break;
		}
	}

	public override void Update() {
		// list files
		// (Keep this code up to date with the render loop)
		var x = r.X1 + 10;
		var y = r.Y1;
		UseButtonFont();
		foreach (var file of files) {
			// 0 height rectangle. We don't know how tall the button will be
			var buttonRect = new Rect(x, y, x + ctx.width, y);
			// now we know the size of the rect
			buttonRect = Button.Update(ctx.WithRect(buttonRect), file.Name, OnClick); 

			y -= buttonRect.Height;

			if (y < r.Y0) break;
		}
	}

	void OnClick(string file) {
		// do something with the file
		...
	}
}
```
There is a lot of code duplication/implicit coupling between the Render and Update functions.
This makes the code harder to maintain and ugly to look at.

If you want something that is easy to read, you will HAVE to do a more retained-mode approach like this instead:

```C#
// Before (alternative). The code looks cleaner ?
class FilePanel : Element {
	File[] files;

	void SetFiles(File[] files) {
		files = newFiles;

		// regenerate file buttons
		Button[] newButtons = new Button<string>[this.file];
		for(var i = 0; i < newFiles.Length; i++) {
			var newFile = newFiles[i];

			var newButton = new Button<string>(newFile.FileName, newFile.FilePath);
			newButton.OnClick += OnSelectFile;

			newButtons[i] = newButton;
		}

		SetChildren(newButtons);
	}

	void OnSelectFile(string filePath) {
		// Do something with the file
		...
	}

	void OnRender() {
		// draw background
		SetFillColor(Colors.Black);
		SetOutlineColor(Colors.White)
		DrawRect(ctx.Rect);
	}

	void OnLayout() {
		LayoutChildrenLinear(Direction.Down);
		LayoutInsetChildren(10);
	}
}

class Button<T> : Element {
	T value;
	string text;
	public Button(string text, T value) {
		this.value = value;
		this.text = text;
	}

	public override void Render() {
		// draw background
		SetFont("source code PRO", 12);
		SetFillColor(Colors.White);
		DrawText(text, 10, 0);
	}

	public override void Update() {
		if (Input.GetMouseClick(Rect)) {
			OnClick?.Invoke(args);
		}
	}
}
```

The code looks cleaner, but it forces you to constantly make your UI more and more granular.
If you wanted to just write out your entire application as a single class with one big function, you can't. 
There is also a bunch of stuff that the Element class is doing under the hood that you don't know about, and can't opt out of.

After the rewrite, we can have immediate mode code like this:

```C#

// ---- After:

class FilePanel {
	public File[] files;

	public void Render(FrameworkContext ctx) {
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
			buttonRect = Button.Render(ctx.WithRect(buttonRect), file.Name, OnClick); 

			y -= buttonRect.Height;

			if (y < r.Y0) break;
		}
	}
}
```

But if we are removing the `Element`, we would need to keep track of the current screen rectangle somehow. 
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
			ctx.Width(ctx.VW * 0.5).Offset(10).Use()	// calling Use() here
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
	RenderContextBuilder Offset() { ... }	// these context resizing and moving functions can return a Builder
}

struct RenderContextBuilder {
	RenderContextBuilder Offset() { ... }
	...

	// Then this builder can return a normal RenderContext, so you can't accidentally pass in a resized render context without 
	// calling Use()
	RenderContext Use() {
		...
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

		
		ctx.Use(); // no mechanism to ensure a user calls this here. before, we enforced it with Render() and AfterRender()

		// render something
	}
}
```

However, it has several advantages. The code is much simpler. There are a lot of hooks that I can remove, because the functionality
exists within the Render() method. And if I need more precise inputs, that can be done in other ways.