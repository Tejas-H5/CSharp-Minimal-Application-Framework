Note: This project is still in development, so it is most likely incomplete. Though in theory it should be cross platform, I haven't tested it on anything other than Windows, and I still have to migrate some APIs to a cross-platform version. 

# Minimal Application Framework

A minimal application framework written in C# with basic support for the bare minimum that I might want in an application, running on .Net Core. It is heavily inspired by Processing, as that is what I was using before this.

## Why?

- Most of my processing sketches only used a few of the features. Namely, text rendering, and line+circle+rectangle rendering. I thought it would be fun to try to implement these from scratch.
- Most of my processing sketches weren't very re-useable, and I struggled to add more lines of code to a sketch after a point. I really wanted the ability for a sketch to be drawn within another sketch, and for both to work just fine.
- I want a way to test/prototype sketches quickly
	
## Where does this project come in?

In MinimalAF, your program is made up of `Element`s (I stole this name from HTML, but the name isn't particularly good, so I might change it later). These are classes that derive from the abstract Element class, and override it's lifetime methods to provide functionality. Basic line/rectangle/circle and text drawing features are supported out of the box, as well as some other features. All elements have a rectangle that defines their position on the screen relative to their parent. This means that your program can be done like a single monolithic processing sketch, or granular to the level of each button being it's own sketch. (I might change the name of `Element` to `Sketch`, because I have noticed I am using that word quite a lot here).

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