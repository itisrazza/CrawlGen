# Crawl Generator

This is a quick tool I've written for generating credits screens ahead of time instead of using the `text()` function again and again in Processing.

It takes in a credits file (`credits.txt` for example) and a config file (`credits.xml`) and generates an image (`credits.png`) which you can then use.

Not fancy by all means but useful for someone like me. 😅

This isn't a project I'm gonna be spending a lot of time on. Unless there's a bit more demand. We'll see.


## Command Line Usage

```
CrawlGen.exe credits.txt
```

## Credits and config files

There's an example in the [Example](Example/) folder.

```
= Star Wars - Episode IV
- Opening Crawl

It is a period of civil war. Rebel
spaceships, striking from a 
hidden base, have won their
first victory against the evil 
Galactic Empire. During the battle,
Rebel spies managed to steal secret
plans to the Empire's ultimate weapon,
the DEATH STAR, an armored space
station with enough power to
destroy an entire planet. 

Pursued by the Empire's sinister agents,
Princess Leia races home aboard her starship,
custodian of the stolen plans that can 
save her people and restore freedom to the galaxy...
```

You can use symbols (like `-` and `=` to demark special lines). Their properties are denoted in the XML config file.

```xml
<?xml version="1.0"?>
<credits width="640" empty-height="24" text-rendering="antialiasgridfit">
  <images>
    <title file="" />
    <footer file="" />
  </images>
  <fonts>
    <font family="Bahnschrift" size="16" />
    <font key="=" family="Noto Serif" size="20" style="Bold, Italic" color="#FFFF00" />
    <font key="-" family="Tahoma" size="7" color="#888888" />
  </fonts>
</credits>
```

Text rendering and font style settings are parsed using [`Enum.TryParse()`](https://docs.microsoft.com/en-us/dotnet/api/system.enum.tryparse?view=netframework-4.8).

Text rendering properties include:
* SystemDefault
* SingleBitPerPixel 
* SingleBitPerPixelGridFit 
* AntiAlias 
* AntiAliasGridFit 
* ClearTypeGridFit 

Font styles include:
* Regular
* Bold
* Italic

And can be merged like "Italic, Bold".
