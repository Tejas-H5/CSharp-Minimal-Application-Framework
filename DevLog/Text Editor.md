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

## Features left to add:

- CTRL + Backspace to erase the  last word [Done, scuffed]
- CTRL + END, HOME  [Done kinda]
    - maybe this can be scoped based. so rather than going to the end of the document, you go to the 
    start of the block, then the previous block, then the start of the document.
- SHIFT HOLD to start select [DONE]
    - ESC to clear the selection
    - should be able to be used with ALL other movement commands, almost no exceptions if possible
    - Funnily enough I implemented this wrong so now typing capital letters gets things selected. It's fixed now I think
- Inserting text should replace the current selection [Ongoing]
    - Looks like this is ongoing sadly. I have to make sure I remove whatever is selected from every method that inserts into the document or deletes.
        - Backspace [DONE]
            - need to make sure I reset the cursor position to the min of the two selected things
        - Normal typing [DONE]
        - Other cases [Haven't found any yet]
- Tab stops of length 4 [Multiple features]
    - going to insert spaces instead of tabs, tabs make moving up and down a line a bit complicated. It can be done, but I have to rewrite a bunch of code, and I would rather do that later tbh
    - When adding \t [Done]
    - when Backspacing whitespace [TODO]
- CTRL + G to go to a line number [Done]
- CTRL + F to start finding stuff, enter to select to the next thing [DOING]
    - CTRL + R to replace

- ALT + LEFT, RIGHT to go back and forth through jump points

- CTRL + up, CTRL + down to move between paragraphs
- CTRL + Z, CTRL + Y to undo and redo
- CTRL + C, CTRL + V to copy paste
- CTRL + SHIFT + C, CTRL + SHIFT + V to copy paste to a buffer that u select with numbers
- CTRL + B to see what u have in the 0 to 9 buffers and edit that stuff before you paste

- CTRL + . to repeat the last thing you did
- CTRL + S to open up the save menu -> 
    - ESC to close
    - CTRL + S to save
- CTRL + P to open
- CTRL + new to make a new file

- 

## Features I've canned

- Movement mode [hold down Ctrl]
    