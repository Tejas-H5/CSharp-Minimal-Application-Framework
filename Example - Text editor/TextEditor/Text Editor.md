# Text editor
Thought I would make an example project using the framework. 
That way, I can make improvements and fixes to the API in realtime.

Also I have been thinking about a text editor that will be better than VIM and VSCode. The main problem with VSCode are:
- Owned by Microsoft
- Movements aren't as fluid as they can be in VIM

But I am using VSCode to type this. That is because:
- minimal configuration and works out of the box
- has LSPs for most langauges, and several thousand easy to install extensions
- I am used to the keybindings
- theme is great
- Its far better than Visual studio for code editing


Why not VIM?
- Too hard to configure
- Can never get it to work just right
- Am too used to VSCode bindings
- I am already pretty fast in VSCode. There aren't things in VIM that make me go "WHOW thats such a cool thing u cant do in VSCode"
- Switching between insert mode and view mode and select mode is a pain
- The buffer system is probably better than the default clipboard but I can't get used to it. In VSCOde, switching between insert mode and view mode involves moving my hands from letters to arrows + home and end. IN vim, I ahve to press I, then Escape . And some basic things like ctrl+backspace, home, end, ctrl+arrows don't work in insert mode. 
- IF I wanted that much configurability over a text editor, I would just code my own

Which is what I am doing now actually. 

## Update 1 week later:

The main point of this project was to improve the framework. There were a bunch of API decisions I had to change and rework to make writing this code easier. But when I work on this text editor now, I am more focused on the problems of coding a text editor, than I am about the problems of rendering text to the screen and what-not. As such, I will stop working on this project for the timebeing, and try making some more example projects to really find where all the pain points in this framework are. So far, we have found:
- Problem: Out of order rendering. Calculating the screen position of the cursor and the highlight regions was tightly coupled to actually rendering the text, but we needed to render the highlights underneath the the text, ideally without using the depth buffer or by using results from the previous frame and having 1 frame where things don't look quite right
    - Solution: Lean further into the IGeometryOutput abstraction, so we can either render directly to the immediate mode context, or to some OpenGL mesh which we can upload to the GPU and render at some point in the future.
- Problem: we can't react to repeating keyboard events, or text inputs
    - Solution: add buffers that are updated every frame that store these

I may decide to do more with this thing in the future?

## Features left to add:

- Tab stops when when Backspacing whitespace [TODO]  
- CTRL + F to start finding stuff, enter to select to the next thing [DOING]    
    - CTRL + F again to reverse, again to close
- ALT + LEFT, RIGHT to go back and forth through jump points
- CTRL + up, CTRL + down to move between paragraphs
- CTRL + Z, CTRL + Y to undo and redo
- CTRL + C, CTRL + V to copy paste
- CTRL + SHIFT + C, CTRL + SHIFT + V to copy paste to a buffer that u select with numbers
- CTRL + B to see what u have in the 0 to 9 buffers and edit that stuff before you paste
- CTRL + S to open up the save menu -> 
    - ESC to close
    - CTRL + S to save
- CTRL + P to open
- CTRL + new to make a new file


## Done

- Inserting text should replace the current selection [Ongoing]
    - Looks like this is ongoing sadly. I have to make sure I remove whatever is selected from every method that inserts into the document or deletes.
        - Backspace [DONE]
            - need to make sure I reset the cursor position to the min of the two selected things
        - Normal typing [DONE]
        - Other cases [Haven't found any yet]

- CTRL + G to go to a line number [Done]
- CTRL + Backspace to erase the  last word [Done, scuffed]
- CTRL + END, HOME  [Done kinda]
    - maybe this can be scoped based. so rather than going to the end of the document, you go to the 
    start of the block, then the previous block, then the start of the document.
- SHIFT HOLD to start select [DONE]
    - ESC to clear the selection
    - should be able to be used with ALL other movement commands, almost no exceptions if possible
    - Funnily enough I implemented this wrong so now typing capital letters gets things selected. It's fixed now I think
- Tab stops of length 4 [Multiple features]
    - going to insert spaces instead of tabs, tabs make moving up and down a line a bit complicated. It can be done, but I have to rewrite a bunch of code, and I would rather do that later tbh
    - When adding \t [Done]  
- New line adds same starting whitespace as previous line [Done]

## Features I've canned

- Movement mode [hold down Ctrl]
- CTRL + R to replace
    - am not sure if this is really the best abstraction
- CTRL + . to repeat the last thing you did