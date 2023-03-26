[ Unresolved ]

# Garbage Collector pauses

The reason I am using C# over something like C++ or Rust is the DX.
It is much easier to program things in C# over Rust, and it is way easier to build things and manage dependencies in C# vs C++. 
Also reflection, it is really handy when you're writing a level editor for a game, or anything else where it may be convenient to know what fields and methods and properties a type has.

But, now I will have to spend the rest of eternity fighting with the garbage collector.

## The main suspect

In one of my 'benchmarking' tests, I have some diagnostic code that I use to write text onto the screen. 
It looks something like this:

``` C#
    ...

    public void Render(FrameworkContext ctx) {
        // some rendering code
        ...

        // lets draw some text that says how we're doing
        string text = "FPS: " + FPS.ToString("0.000") +
            "\nLines drawn: " + amount +
            "\nCapType: " + CapType.ToString() +
            "\nVertex refreshes: " + CTX.TimesVertexThresholdReached +
            "\nIndex refreshes: " + CTX.TimesIndexThresholdReached +
            "\nVertex:Index Ratio: " + CTX.VertexToIndexRatio;

        ctx.DrawText(text, 10, ctx.VH - 50);
        
    }

```

TBH none of those metrics really matter, I should have been printing the vertex count and index count directly but that is a separate tangent. 
What I believe is happening is that each time this loop runs, a new string is being allocated each time, and is never de-allocated.
When I check the internet.

I've taken a look at ZString, and it seems like it wants to be all things for all people. 
That library alone probably has more lines of code in it than my entire framework. 
And the way they implemented the string.concat function is by using some sort of codegen technique to make methods like `string Concat<T1, T2>()`, `string Concat<T1, T2, T3>()`, so on and so forth, all the way up to T16. 
But what if I wanted to concatenate 17 strings with zero allocations? now what?
It does seem 2-3 times more performant though. 
Still, am not sure if it is worth adding this dependency.

The string.Create method seems to also be a thing, but once the string is made, I can't overwrite the existing memory.
If I had some way of making mutable strings, my problems will be solved. 
What if I made a new string class that was just a char array?

``` C#
struct MutableString { 
    char[] letters; 
    int pos = 0;

    public MutableString(len) {
        letters = new char[len];
    }

    public void Append(const string& text) {
        // this still allocates every now and then:
        // if (text.Length + pos > letters.Length) { resize letters to letters.Length * 2 or text.Length + pos + letters.Length or some other algorithm similar to c++'s vector, but keeping in mind we need to fit all the new text in 'text'   }
        // copy text into letters from pos onwards
    }

    private void Resize(wantedSize) {
        ...
    }

    public void Append(const double d, int decimals) { ... }
    public void Append(const float d, int decimals) { ... }
    public void Append(const int d) { ... }
    public void Append(const uint d) { ... }
    public void Append(const byte d) { ... }
    public void Append(const char d) { ... }
    ... etc ...
}

```

The Append method would still cause an allocation, but if this buffer was constantly being cleared, and a similar amount of text was being rewritten to it each frame, eventually we would reach zero allocations, which should stop or significantly reduce GC pauses in general.
We can then convert all text methods in our 'framework' to operate on char[].
However, if the string being inserted into Append isn't a compile time constant, it is being allocated anyway, and this defeats the purpose of our new struct.
I will trust that the devs using this know what they are doing.

I wonder if we can overload << like in C++ ? Looks like we can override the left shift operator <<, but it has to be a function like (T, int), so we can't. Will stick with 'Append()' for now.