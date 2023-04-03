These are mainly my findings when trying to make a text editor with the framework.
The audio part will be evaluated when I try making a DAW.

# Current issues with the framework

## Text
- Tab width has been hardcoded to 4. 
    - Need a CTX API to say SetTabWidth(x) and GetTabWidth(). 
    - I probably don't want to couple MutableString to this. 
        - Dont think I even need to, all the "moveCursorUpALine" and such methods are in my TextBuffer class which won't be in MinmalAF, so probably isn't a risk atm.
- We need bold and italic for text
- We need to move text to signed distance fields
    - We are making an entirely new font atlas for characters with a different size, not ideal. 
    We may want to move to using just 1 medium/large texture or SDF or do some sort of mim-mapping style image pyramid type thing
- Make sure the new font is being disposed of properly [todo...]
    - Still TODO, but I think its working. will test later
- I keep forgetting to switch to a null texture before rendering text.
    - but I'd rather not add if statements to unset the text texture. 
    - It may be worth moving to the mesh-vector method sooner than later.


## Audio
- Audio classes are all badly named
    - the names are retained from an old codebase, we should rename them
- Right now, an AudioInputSource thinggy that streams a thing can only be used as an input into a single audio source at a time
    - AudioSourceInput needs to stop being the thing that is played, and rather an object that can create a new instance of a thing that can be played. One shot clips can just point straight back to the object itself without allocating anything, and audio streamed sources can make a sepearte stream for each audio clip. 
        - Is this the factory pattern? 
            - Who cares

