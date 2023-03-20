These are mainly my findings when trying to make a text editor with the framework.
The audio part will be evaluated when I try making a DAW.

# Current issues with the framework

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


- Tab width has been hardcoded to 4. 
    - Need a CTX API to say SetTabWidth(x) and GetTabWidth(). 
    - I probably don't want to couple MutableString to this. 
        - Dont think I even need to, all the "moveCursorUpALine" and such methods are in my TextBuffer class which won't be in MinmalAF, so probably isn't a risk atm.

- We need bold and italic for text
- We need to move text to signed distance fields

- I keep forgetting to switch to a null texture before rendering text.
    - but I'd rather not add if statements to unset the text texture. 
    - It may be worth moving to the mesh-vector method sooner than later.

# not really an issue / solved

- InternalFontAtlasTexture
    - Solved, but now you can do SetTextureToCurrentFontTexture() and then GetTexture() to get the internal font texture. Eh why not
    - Nah I don't like the assymetry. I just renamed InternalFontAtlasTexture to CurrentFontTexture. We are simply exposing this variable as public, why not (its a getter, so it isn't like something else can overwrite it)

- Need min() and max() and lerp() a lot. Probably a bunch of other math stuff
    - Added MathHelpers with some commonly used math stuff I use a bunch in basically every project I work on