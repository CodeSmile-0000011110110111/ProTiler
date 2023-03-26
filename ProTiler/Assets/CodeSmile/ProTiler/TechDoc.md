# Tech doc

## Design Goals
- no fixed map sizes, expand as you draw
- chunkable pieces, configurable chunk size
- world design, not just maps
- placement & overwrite rules
- customizable export format
- async, bursted runtime loader

### for laters ...
- support grid-based pathfinding
- support hex tiles


## Assume drawing with prefabs:
I need to store the tiles.

Possible tile sources:
- prefabs
- meshes
- game objects (convert existing)
- generated meshes ? (=> saved as asset anyway, right?)

...

- need a "tile set" of prefabs (reference to each prefab used in the world)
- for each tile layer: store reference to "tileset" prefab
  - how? guid, name (brittle)
  - ((opposite: for each tileset prefab, store a list of coordinates (+ layer) where that tile is used --- stupid: due to enumeration/iteration))


### Question: tilesets per layer or for entire tilemap?
- per layer:
  - duplicate info stored across layers
  - allows me to quickly assess which tile prefabs are in use on a given layer
  - allows for custom tile settings per layer, even for the same tile prefab (tree in layer A can have different properties than tree in layer B, eg blocking vs non-blocking, convert to gameobject, etc)

### What's in a layer? What defines a layer?
- Tilemap provides default grid settings (x/z and y)
- each layer can override the grid settings (varying grid settings)
- layer could be non-grid (free placement)
- layer types:
  - tiles
  - terrain (poly terrain)
  - gameobject (free placement, randomize)
  - info (collisions, triggers, lighting, etc) (meaningful??)
  - path layer (road, river, rail, race tracks, assembly lines, pipes, tunnels, ...) (not: bezier)

### Idea: a tile can be a layer itself
- subdivide a tile into a smaller grid
- needs: way to reference that layer from a tile

### Question: what if i want to animate a tile?
- it is not a gameobject so ... ? simple transform anim is doable
- everything else is animatin ON that tile (eg trees in the wind)

### Question: should I have a single class with self-ref to go ever deeper (tile layer within tile layer within...)?


class TileWorld
    TileMap[] chunks;
    ChunkSize

class TileChunk
    TileWorld owner;
    TileLayer[] layers;
    TileGrid grid;

class TileLayer
    TileChunk owner;
    Guid[] tiles;
    GameObject[] tileset;

class Coords
    convert from mouse to world to grid pos
        "perform picking"

class TileGrid : GridLayout
    size
    //swizzle (xz)
    //layout (rect)
    //gap (0)