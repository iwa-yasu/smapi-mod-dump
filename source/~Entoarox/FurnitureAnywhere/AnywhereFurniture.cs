using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;

using Entoarox.Framework;

namespace Entoarox.FurnitureAnywhere
{
    public class AnywhereFurniture : Furniture, ICustomItem
    {
        /*********
        ** Public methods
        *********/
        public AnywhereFurniture() { }

        public AnywhereFurniture(Furniture item)
            : base(item.ParentSheetIndex, item.TileLocation)
        {
            this.defaultBoundingBox.Value = item.defaultBoundingBox.Value;
            this.boundingBox.Value = item.boundingBox.Value;
            this.currentRotation.Value = item.currentRotation.Value;
            this.rotations.Value = item.rotations.Value;
            this.sourceRect.Value = item.sourceRect.Value;
            this.updateDrawPosition();
        }

        public Furniture Revert()
        {
            Furniture self = new Furniture(this.ParentSheetIndex, this.TileLocation);
            self.defaultBoundingBox.Value = this.defaultBoundingBox.Value;
            self.boundingBox.Value = this.boundingBox.Value;
            self.currentRotation.Value = this.currentRotation.Value;
            self.rotations.Value = this.rotations.Value;
            self.sourceRect.Value = this.sourceRect.Value;
            self.updateDrawPosition();
            return self;
        }

        public override bool isPassable()
        {
            return this.furniture_type.Value == 12;
        }

        public override string getCategoryName()
        {
            return "FurnitureAnywhere";
        }

        public override bool performObjectDropInAction(Item dropIn, bool probe, Farmer who)
        {
            return false;
        }

        public override bool canBePlacedHere(GameLocation l, Vector2 tile)
        {
            for (int index1 = 0; index1 < this.boundingBox.Width / Game1.tileSize; ++index1)
            {
                for (int index2 = 0; index2 < this.boundingBox.Height / Game1.tileSize; ++index2)
                {
                    Vector2 key = tile + new Vector2(index1, index2);
                    if (l.Objects.ContainsKey(key))
                    {
                        if (l.objects[key] is Furniture)
                        {
                            Vector2 vector2 = key * Game1.tileSize - new Vector2(Game1.tileSize / 2);
                            Furniture furniture = (Furniture) l.objects[key];
                            if (furniture.furniture_type.Value == 11 && (furniture.getBoundingBox(furniture.TileLocation).Contains((int) vector2.X, (int) vector2.Y) && furniture.heldObject.Value == null && this.getTilesWide() == 1))
                                return true;
                            if ((furniture.furniture_type.Value != 12 || this.furniture_type.Value == 12) && furniture.getBoundingBox(furniture.TileLocation).Contains((int) vector2.X, (int) vector2.Y))
                                return false;
                        }
                        return false;
                    }
                }
            }
            if (this.ParentSheetIndex == 710 && l.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Water", "Back") != null && (!l.objects.ContainsKey(tile) && l.doesTileHaveProperty((int)tile.X + 1, (int)tile.Y, "Water", "Back") != null) && l.doesTileHaveProperty((int)tile.X - 1, (int)tile.Y, "Water", "Back") != null || l.doesTileHaveProperty((int)tile.X, (int)tile.Y + 1, "Water", "Back") != null && l.doesTileHaveProperty((int)tile.X, (int)tile.Y - 1, "Water", "Back") != null || (this.ParentSheetIndex == 105 && this.bigCraftable.Value && (l.terrainFeatures.ContainsKey(tile) && l.terrainFeatures[tile] is StardewValley.TerrainFeatures.Tree) && !l.objects.ContainsKey(tile) || this.name != null && this.name.Contains("Bomb") && (!l.isTileOccupiedForPlacement(tile, this) || l.isTileOccupiedByFarmer(tile) != null)))
                return true;
            return !l.isTileOccupiedForPlacement(tile, this);
        }

        public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
        {
            Point point = new Point(x / Game1.tileSize, y / Game1.tileSize);
            this.TileLocation = new Vector2(point.X, point.Y);
            if (this.furniture_type.Value == 6 || this.furniture_type.Value == 13 || this.ParentSheetIndex == 1293)
            {
                Game1.showRedMessage("Can only be placed in House, Cabin or Shed");
                return false;
            }
            for (int index1 = point.X; index1 < point.X + this.getTilesWide(); ++index1)
            {
                for (int index2 = point.Y; index2 < point.Y + this.getTilesHigh(); ++index2)
                {
                    if (location.doesTileHaveProperty(index1, index2, "NoFurniture", "Back") != null)
                    {
                        Game1.showRedMessage("Furniture can't be placed here");
                        return false;
                    }
                    if (location.getTileIndexAt(index1, index2, "Buildings") != -1)
                        return false;
                }
            }
            this.boundingBox.Value = new Rectangle(x / Game1.tileSize * Game1.tileSize, y / Game1.tileSize * Game1.tileSize, this.boundingBox.Width, this.boundingBox.Height);
            foreach (Farmer character in location.farmers)
            {
                if (character.GetBoundingBox().Intersects(this.boundingBox.Value))
                {
                    Game1.showRedMessage("Can't place on top of a person.");
                    return false;
                }
            }
            foreach (KeyValuePair<Vector2, Object> i in location.objects.Pairs)
            {
                if (!(i.Value is Furniture))
                    continue;

                Furniture furniture = (Furniture)i.Value;
                if (furniture.getBoundingBox(furniture.TileLocation).Intersects(this.boundingBox.Value))
                {
                    Game1.showRedMessage("Furniture can't be placed here");
                    return false;
                }
            }
            this.updateDrawPosition();
            if (!this.performDropDownAction(who))
            {
                Object @object = (Object)this.getOne();
                @object.shakeTimer = 50;
                @object.TileLocation = this.TileLocation;
                @object.performDropDownAction(who);
                if (location.objects.ContainsKey(this.TileLocation))
                {
                    if (location.objects[this.TileLocation].ParentSheetIndex != this.ParentSheetIndex)
                    {
                        Game1.createItemDebris(location.objects[this.TileLocation], this.TileLocation * Game1.tileSize, Game1.random.Next(4));
                        location.objects[this.TileLocation] = @object;
                    }
                }
                else
                    location.objects.Add(this.TileLocation, @object);
                @object.initializeLightSource(this.TileLocation);
            }
            Game1.playSound("woodyStep");
            return true;
        }

        public override Item getOne()
        {
            AnywhereFurniture furniture = new AnywhereFurniture(this);
            furniture.drawPosition.Value = this.drawPosition.Value;
            furniture.defaultBoundingBox.Value = this.defaultBoundingBox.Value;
            furniture.boundingBox.Value = this.boundingBox.Value;
            furniture.currentRotation.Value = this.currentRotation.Value;
            furniture.rotations.Value = this.rotations.Value;
            furniture.sourceRect.Value = this.sourceRect.Value;
            return furniture;
        }

        public override bool clicked(Farmer who)
        {
            try
            {
                Game1.haltAfterCheck = false;
                if (this.furniture_type.Value == 11 && who.ActiveObject != null && this.heldObject.Value == null)
                    return false;
                if (this.heldObject.Value == null && !(who.ActiveObject is Furniture))
                    return who.addItemToInventoryBool(this.Revert());
                if (this.heldObject.Value == null || !who.addItemToInventoryBool(this.heldObject.Value))
                    return false;
                this.heldObject.Value.performRemoveAction(this.TileLocation, who.currentLocation);
                this.heldObject.Value = null;
                Game1.playSound("coin");
                return true;
            }
            catch
            {
                Game1.showRedMessage("Broken furniture detected, deleting...");
                Game1.currentLocation.objects.Remove(this.TileLocation);
                return true;
            }
        }
    }
}
