Note: This project is still in development, so it is most likely incomplete. It is basically done though, but there may be some blind spots, especially with shaders and UI in general. Once I have actually made something cool with it, I will say that it is finished.

# Minimal Application Framework

A minimal application framework written in C# with support for the bare minimum that I want in an application framework. It runs on .Net core, because I like C# and it's ecosystem.

## Why?
I really liked programming small programs in Processing with a minimalist UI made entirely of lines, rectangles, circles and Consolas font text because that was the extent of my UX knowledge. And I actually liked the way this looked. But here are the problems I encountered with this approach:
- But I often had trouble sending my program to other people because my 100 line processing sketch would compile into a 178 mb binary that included all of java + processing, or a 3 mb binary that just wouldn't run at all because they didn't have the correct Java. Hopefully by switching to .net, I can avoid this problem because Microsoft will be shilling .net and eventually shipping windows with it built in.
- Most of my processing sketches weren't very re-useable, and I struggled to add more lines of code to a sketch after a point. I really wanted the ability to compose sketches with other sketches, as this would have made writing a lot easier.
- I also want a way to test/prototype sketches and algorithms quickly, possibly in a parameterized way
	
## Where does this project come in?

In MinimalAF, your program is made up of `Element`s, which are basically standalone programs that can optionally have other elements within them as children.

Your program can be a single element, similar to a  monolithic Processing sketch, or be composed of several granular elements, to the point where each button is it's own element. Both are valid ways of structuring your code (and if you are making anything needing complex and repeating GUI, this is probably the only way to go), although there are some more conveniences you will have by having some granularity.

### Testing
This framework comes with a rudimentary testing harness. In order to make a testcase, you would create your test scenario as it's own Element, and then mark it with a `VisualTest` attribute, like so:

```c#
[VisualTest(
	description: @"Text rendering test. There should be some text in the center of the screen saying Hello There",
	tags: "2D, text"
)]
class TextTest : Element {
	public override void OnMount(Window w) {
		w.Size = (800, 600);
	}

	public override void OnRender() {
		SetFont("Consolas", 16);
		SetDrawColor(0, 0, 0, 1);

		DrawText("Hello there", VW(0.5f), VH(0.5f), HAlign.Center, VAlign.Center);
	}
}

```

And then your `Main` should be something like this:

```c#
class Program {
	static void Main(string[] args) {
		var testRunner = new VisualTestRunner();
		new ApplicationWindow().Run(testRunner);
	}
}
```

`VisualTestRunner` uses System.Reflection to find and collate all Elements in the project tagged with the VisualTest attribute.
If your test has public properties, you should also be given some UI that can change these properties in realtime.
If your test has a constructor that accepts parameters, you should also have some UI that can restart the test with different starting parameters.

(I stole this idea from osu! framework, but they've done it much better and they have dynamic compilation as well somehow. I will have to read their code and reverse engineer it at some point. Or maybe you are reading this and you know how it's done, some pointers on how to get this would be appreciated)

## What would like to add but may never add as I can't be bothered:
(help/PRs/suggestions welcome)
- Improve text rendering. The text rendering is looking jankyAF
- Improve testing harness
	- Dynamic compilation for visual tests, 
	- Add custom UI and UI overrides for the test harness
	- Improve test organization and searching
	- Make those menus hideable
	- Add a fullscreen mode to tests
	- Add some way to see the element tree and toggle Debug mode for elements mid-test
	- Add assertions, and unit testing capabilities to VisualTests