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
Not all features are listed here, I only started making this list after I had a bunch of them in already.


- Line numbers [DONE]
- Keep the current line in the vertical center at all times [DONE]
    - feature is harder than I thought. I will need some way to know how far down in the document the cursor is.
    Word wrap is what makes this hard. 
    I guess the easy way would be to just render every letter. And then just use like some sort of offset to get the cursor where
    it needs to be. I will try that for now, and move to something else
- Typical movement (These movements work on literally every text input in the operating system, and even across operating systems I think. Its a bit silly to not have these in some way or another)
    - CTRL + backspace to delete a whole world
    - CTRL + delete to delete the next word
    - DELETE to delete the next char
- Movement mode [hold down Ctrl]
    - Jump to the end of the line [E]
    - Jump to the start of the line [Q]
    - CTRL + F to enter 'find mode' 
        - CTRL + F + A to enter reverse find ?
        - toggle case sensitive, whole word
            - CTRL + Q in find mode to toggle case sensitive
            - CTRL + W in find mode to toggle whole word
        - Type something, and we get matches for it
        - 
- Command mode [ CTRL + SPACE ]
    - Save file
    - Run terminal command ? (idk if this is hard or not. maybe not if it's too hard)
    - CTRL + J, K, L, ; -> Go to editor
    - CTRL + P
        - open file in folder
     
- Select mode [hold down CTRL + SHIFT]
    - pressing shift highlights from the start of the move to the end
    - Backspace deletes the selection


