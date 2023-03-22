These are mainly my findings when trying to make a text editor with the framework.
The audio part will be evaluated when I try making a DAW.

# Current issues with the framework

- Tab width has been hardcoded to 4. 
    - Need a CTX API to say SetTabWidth(x) and GetTabWidth(). 
    - I probably don't want to couple MutableString to this. 
        - Dont think I even need to, all the "moveCursorUpALine" and such methods are in my TextBuffer class which won't be in MinmalAF, so probably isn't a risk atm.

- We need bold and italic for text
- We need to move text to signed distance fields
    - We are making an entirely new font atlas for characters with a different size, not ideal. 
    We may want to move to using just 1 medium/large texture or SDF or do some sort of mim-mapping style image pyramid type thing

- I keep forgetting to switch to a null texture before rendering text.
    - but I'd rather not add if statements to unset the text texture. 
    - It may be worth moving to the mesh-vector method sooner than later.

# not really an issue / solved

- InternalFontAtlasTexture
    - Solved, but now you can do SetTextureToCurrentFontTexture() and then GetTexture() to get the internal font texture. Eh why not
    - Nah I don't like the assymetry. I just renamed InternalFontAtlasTexture to CurrentFontTexture. We are simply exposing this variable as public, why not (its a getter, so it isn't like something else can overwrite it)

- Need min() and max() and lerp() a lot. Probably a bunch of other math stuff
    - Added MathHelpers with some commonly used math stuff I use a bunch in basically every project I work on



- There might need to be a way to render something to buffer mesh outputs separately. Currently I would use a list to do something like this. 
    - E.g, when I am rendering all the text in the text file, I need to render the highlighted region underneath the text. The problem is that  would be calculating the screenspace coordinates of these highlighted regions within the for-loop. So I can't render it underneath the text. So I just push the highlighted spans onto a list and render it in the next frame, right? (For the sake of this example, I will ignore using the z-buffer + some cutout shader and rendering the highlights literally further away)
        - It would be nicer if I could do something like this:
            SetMeshBuffer(1)
            DrawText()

            SetMeshBuffer(2)
            DrawText()

            DrawMeshBuffer(2)
            DrawMeshBuffer(1)

            Or maybe:
            _mb1 = new MeshBuffer();
            _mb2 = new MeshBuffer();

            void Render() {
                MakeText(_mb1)
                MakeHighlights(_mb2)

                SetDrawColor(blue)
                _mb2.Draw();

                SetDrawColor(black)
                _mb1.Draw();
            }

            It can also help when we might want to batch the rendering of things with the same colors,
            or if we want to make it easier for people to memoize mesh generation. 
            Would def be a nice switch up to the API I think. 

            To be clear, this is a different API. It is no longer a buffered mesh output, but a mesh builder type of API. Like std::vector but for meshes. 

    - To help fix this issue (As I believe doing so will provide large DX wins and get us 1 step closer to finishing this API and porting to Rust), I want to make the following additions and changes:
        - Fonts should be individual objects that users create. i.e
            Font _font = new Font("Source code PRO", 24);
            - And then if they want to draw text, rather than doing ctx.DrawText, they do _font.DrawText().
                - Imagine drawing one font using the texture from another font lmao
                    - will totally doable in this framework. We only provide the sharpest of pointy edges
            - I want to do something similar for most drawable objects.
        - I want a secondary mesh output stream. MeshBuilder. Mesh. DynamicMesh. ResizingMesh. DynMesh. Idk what a good name is. I will just call it MeshBuilder for now, since it is similar in principle to a stringbuilder.
        - Basically, we can redirect our geometry output calls to a MeshBuilder instead of a BufferedMeshOutput, in order to progressively build a mesh. Then we call UploadToGPU() to update the data on the GPU. This will get called automatically if we call Draw(). The only thing I am finding hard to define is the automatic resizing rules that it might have. Should the buffers double in size each time we run out of vertices/indices like std::vector? Should we only allocate Max(what as we need, what we have already got) each time? Maybe as the number of vertices gets higher and higher, the chance of reaching that amount halves, so it would be just as efficient to increase the size by a constant factor? I'm not sure. For now I will hardcode the doubling rule, although in the future I may allow specifying functions to calculate the new size of a buffer each time we want to call UploadToGPU().
        - It looks like my framework isn't very minimal actually. There are a lot of places where I am trying to overgeneralize where I shouldn't be. And there are a lot of places where I am using interfaces where I could be using function pointers or `Func<T>` or `Action<T>`
        - It looks like I have some other MeshBuilder class, but it doesn't have any mechanism to rebuild a mesh in-place. It is getting deleted as well
    - I really don't want to have two different ways of drawing though, because then peolpe will need to memorize two seperate but linked APIs. E.g I don'tw ant to have IM.Rect(ctx, rect) and IM.Rect(ctx.MeshOutput, rect) as the two equally valid but different ways to draw. It also doesn't make sense for appending vertices onto a plain c# object to be coupled to FrameworkContext ctx though, so I think I will actually just completely move to a convention like IM.NameOfThingWeWantToDraw(output, ...params). Then if we want normal immediate mode behaviour, we do IM.Rect(ctx, rect) and if we want to draw this rect to a mesh buffer, we do IM.Rect(_buffer, rect); and then call _buffer.Draw() when we want to draw the thing.
        Frameworkctx will have to inherit from IGeometryOutput and wrap some calls to the buffered mesh output, but I think it will look better than IM.Rect(ctx.BufferedMeshOutput, rect) each time;

    - Final steps:
        - set IVertexcreator2D _vertexCreator to something [DONE]
        - make _font.Draw(font, ) set the texture [done]
        - Make sure the new font is being disposed of properly [todo...]

    - this is almost solved :0
