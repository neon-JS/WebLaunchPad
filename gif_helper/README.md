# gif_helper

## About
Converts a landscape GIF into an _8px * 8px_ framed GIF. Starts with the first _8px * 8px_ and
moves _1px_ to the right for each new frame.
This can be used to create e.g. color gradient effects without having to create a bunch of
frames. Instead you can create one wide (_8px * \_\_\_px_) file and use this tool.
The resulting gif can be sent to the Launchpad via API.

## Usage
```bash
./gif_helper <input file> <output file>
# e.g. ./gif_helper ../my_large.gif framed.gif
```

## Releases
No such thing as releases. But for convenience, there are prebuilt binaries in the _out/_ folder.