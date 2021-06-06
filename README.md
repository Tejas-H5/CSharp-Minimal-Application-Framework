# CSharp Minimal Application Framework
A minimalist application framework in C# with basic support for most things you would want in an application, running on .Net Core. (Linux, Mac, Windows).
The name Minimal Application Framework was a little too verbose in code, so the namespace has been shortened to MinimalAF.

I initially wanted to just implement immediate mode rendering, but I thought I might as well implement all the other stuff that most of my programs will need as well.
I am constantly maintaining it because I use it in my other projects.

## Dependencies

- C#, .Net Core
- OpenTK
	- OpenGL and OpenAL
- NAudio
	- Just classes for loading audio files which in theory should be fully .Net Core compatible (e.g the  MediaFoundationReader class)


## Features:

- All immediate mode functions from Processing that I use frequently, as well as some of my own
	- mainly line, circle, rectangle and text drawing, and some basic stencilling
- A simple component-based UI system inspired by the Unity RectTransform model
	- I have tried to make generating UI Heirarchies through code as painless as possible but it is probably still a little painful at the moment. I might document some ways of doing this or create some example projects
- Mouse and keyboard input, as well as dragging logic and typing input logic out of the box
- A very simple observer pattern implementation that uses non-recursive events (a normal c# event with a boolean guarding against invoking the event a second time)
- Basic audio playing and generating capabilities


## Near future features:
- 3D immediate mode rendering

## Distant future features:
- Framebuffers and Post-processing
- Split-screen rendering out of the box
- Animation capabilities similar to those found in the 'Motion periodic table' (http://foxcodex.html.xdomain.jp/)



### Unrelated 
In earlier versions of this Readme, I contemplated adding my own physics engine, as well as entity component system support.
I now realize that this is quite unrealistic, and that those things would be better off as their own projects simply taken from a 3rd party, since they would unnecessarily bloat this library. 
As for ECS, it looks very promising. If I were to make a physics engine, I would try to make it somehow support both ECS as well as a scene-graph approach.
Most likely I would just use something that already exists though.