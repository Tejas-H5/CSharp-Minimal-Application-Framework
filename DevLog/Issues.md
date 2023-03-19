These are mainly my findings when trying to make a text editor with the framework.
The audio part will be evaluated when I try making a DAW.

# Current issues with the framework

- There might need to be a way to render something to buffer mesh outputs separately. Currently I would use a list to do something like this. 
    - E.g, when I am rendering all the text in the text file, I need to render the highlighted region underneath the text. The problem is that  would be calculating the screenspace coordinates of these highlighted regions within the for-loop. So I can't render it underneath the text. So I just push the highlighted spans onto a list and render it in the next frame, right? (For the sake of this example, I will ignore using the z-buffer + some cutout shader and rendering the highlights literally further away)
        - It would be nicer if I could do something like this:
            

# not really an issue

- You can do SetTextureToCurrentFontTexture() and then GetTexture() to get the internal font texture. Eh why not

