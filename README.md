# CSharp-Minimal-Rendering-Engine
A minimalistic program framework in C# with basic drawing, text rendering and input support.

I initially wanted to just implement rendering, but I thought I might as well implement all the other stuff that most of my programs will need as well.
I am constantly maintaining it while I use it in my other projects.

The project aims to combine:

- All usefull immediate mode functions from Processing, as well as some of my own
- A component based UI system that also uses the RectTransform UI model from Unity
- Mouse and keyboard input, as well as dragging logic and typing input logic out of the box
- A very simple observer pattern implementation that uses non-recursive events (a normal c# event with a boolean guarding against invokeing the event a second time)

It also comes with a very simple UI editor that can be used to generate UI heirarchies relatively quickly.
I might add more external tools if needed.


All of the windowing, input and OpenGL rendering has been implemented with OpenTK


### Future plans
- 3D retained mode rendering
- Split-screen rendering

### Things that I might add
- Physics Engine
- Entity Component System support


Also the reason why I am contemplateing adding my own physics engine is because I am frustrated with Unity's own physics engine.
If I wanted to make a platformer game where every block is a square tile, then the edge where two tiles perfectly touch
will generate collisions with any character that goes overtop it. The same thing applies in 3D, if someone wanted to make
a Trackmania-like game with a level editor for example.
And it would be leveraging the ECS design, as I have heard that it is significantly faster than the traditional scene-graph due
to all of the data being held contiguously.
The only thing I can't seem to figure out about the entity component system is how to enforce object heirchies like in the scene-graph model 
without resorting to a bunch of pointers to other objects, essentially resulting in the scene-graph. 
There should definaltey be a way though.

Although I doubt I can call this a minimalist rendering framework anymore after I add those.
If I find that ECS can be added through an external nuget package or the like, I will do that instead.

All of these features may be optionally added through 3rd party packages.
