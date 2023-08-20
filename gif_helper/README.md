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

## Docker
```bash
docker pull ghcr.io/neon-js/gif_helper:main
docker run -v $(pwd):/gifs ghcr.io/neon-js/gif_helper:main gif_helper /gifs/<input file> /gifs/<output file>
# e.g. docker run -v $(pwd):/gifs ghcr.io/neon-js/gif_helper:main gif_helper /gifs/my_large.gif /gifs/framed.gif
```