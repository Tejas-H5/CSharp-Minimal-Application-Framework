# CSharp Minimal Application Framework
Note: This project is still in development, so it is most likely incomplete.

A minimal application framework in C# with basic support for the bare minimum that you might want in an application, running on .Net Core. (Linux, Mac, Windows).
The name Minimal Application Framework was a little too verbose in code, so the namespace has been shortened to MinimalAF.

I initially wanted to just implement immediate mode rendering, but I thought I might as well implement all the other stuff that most of my programs will need as well.
I am constantly maintaining it while I use it in my other projects.

## Dependencies

- C#, .Net Core
- OpenTK
	- OpenGL and OpenAL
- NAudio
	- Just classes for loading audio files which in theory should be fully .Net Core compatible (e.g the  MediaFoundationReader class)


## Features:

- All immediate mode functions from Processing that I use frequently, as well as some of my own
	- mainly line, circle, rectangle and text drawing, and some basic stencilling
	- Poly-line drawing
- A simple component-based UI system inspired by the Unity RectTransform model
	- I have tried to make generating UI Heirarchies through code as painless as possible but it is probably still a little painful at the moment. I might document some ways of doing this or create some example projects in the future when it's finished
- Mouse and keyboard input, as well as dragging logic and regular typing input logic out of the box
- Basic audio playing and generating capabilities


## Distant future features:
- Full support for 3D rendering, Framebuffers and Post-processing
- Split-screen rendering out of the box
- Animation capabilities similar to those found in the 'Motion periodic table' (http://foxcodex.html.xdomain.jp/)



## Why am I making this?

Mainly get better at software development, and for use in my own personal projects. There are certain capabilities that I have added to this framework (like easy immediate mode drawing of poly-lines, stencilling, Unity-style UI, and precise audio playback) that aren't in other frameworks and vice-versa.