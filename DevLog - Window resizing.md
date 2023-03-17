NOTE: I am currently not working on this, and no solution was found yet

# Window resizing

Currently, I am running into a problem where a simple game loop will not redraw itself when the window is being resized.
This is usually not much of a problem, but it often useful to see how an app will resize itself based on size.
And if you are making a game, you probably don't want resizing to be a way that players can slow down the game.

Tried using the GLFW refresh callback, but it only rerenders when you are dragging the resize handle, but if you just have your mouse down, it is the same problem as before. People on the internet are saying to use a separate render thread, and it will work just fine.

However, this will make it much harder to have a simplistic API that I have now:

``` C#
// Main thread

class EntryPoint : IRenderable {
    // update and render happens here
    public void Render(FrameworkContext ctx) {
        // events + rendering here, so immediate mode API works easily
    }
}
```

Under the hood, it is ran like this:

```
ProgramWindow(iRenderable)
    -> Main thread
        -> While (!window.ShouldClose): 
            processEvents();
            iRenderable.render(newContext(window));
            GlobalCTX.SendBufferedMeshOutputToGPUAndDraw()
            waitTillNextFrame();

iRenderable.render(ctx):
    ctx.variousDrawCalls():
        -> check current input state
        -> set some GPU state, like GLEnabledCaps, current shader, clear color etc.
        -> push vertices to a buffered mesh output
            GlobalCTX.SendBufferedMeshOutputToGPUAndDraw() if we don't have enough vertices or indices or when global state changes, so it feels like immediate mode
```

Some of these things will need to move to a seperate thread. 
If I move rendering to a separate thread, then the input state will be out of sync with the render thread.
And, I can't actually poll for input on the non-main thread.

I can think of 4 options that allow us to keep the simple API:
- Rerender using GLFW's OnRefresh callback, and accept that we can't rerender while resizing properly
    - Actually not as bad a solution as it sounds. Very easy to implement, and solves the main issue with not being able to rerender while resizing - being that you can't see how the ui elements on the window resize until you stop resizing. Allowing the program to update while you have resize pressed is a niche edge case, and would be nice to have but isn't valuable enough to put hours of effort into at this stage
    - Freezing the app timer here might be a good idea

- Make a separate thread for rendering, run the Render() function there, and sync events with that thread somehow
    - In the render thread: 
        - method 1:
            - wait for window to poll events;
                // But then if we are resizing, won't this be blocked?
                // Yes it will, and there is no good way to unblock while resizing
            - renderable.Render(); // should work as normal 
        - method 2:
            - renderable.Render() as normal;
            - render thread can pull input state from a shared memory buffer using non-blocking producer/consumer approach. 
            - This might be the simplest solution actually    
            - However, I am encountering issues where a lot of the GLFW windowing methods can only be called via the main thread. They will need to be synchronized via p/c. 
            - This approach ends up being kind of funny, in that we are using 2 threads but we don't get any performance benefits because we are blocking for inputs.

- Make a seperate thread for rendering, run the Render() function on the main thread and send rendering commands to that other thread somehow
    - share a bunch of rendering primitives with the render thread using a producer/consumer approach
    - 

- Alternative idea: start a new render thread and run Render() from there just for when we are resizing without syncing any input state, and then destroy it as soon as we aren't resizing any more. It's not like we need to process inputs while we are resizing the window
    - Seems this might be very hard, because OpenGL wants you to only make OpenGL calls on the thread where the context was created.
    - 

