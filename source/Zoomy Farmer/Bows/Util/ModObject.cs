﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using StardewValley.Objects;
using PyTK.Extensions;
using PyTK.CustomElementHandler;
using SObject = StardewValley.Object;
using StardewValley;

namespace GekosBows {

	internal class ModObjectData {
		public string ObjectID = "ModObjectID";
		public string Name = "ModObjectName";
		public string Description = "ModObjectDescription";
		public string ObjectType = Constants.TYPE_CRAFTING;
		public int ObjectCategory = Constants.CATEGORY_JUNK;
		public int Price = 1;
		public string Crafting = null;
		public int Edibility = -300;

		public ModObjectData() {
		}

		public ModObjectData(string ObjectID, string Name, string Description) {
			this.ObjectID = ObjectID;
			this.Name = Name;
			this.Description = Description;
		}

		public CustomObjectData CreateObjectData(Texture2D texture, Type type, int index = 0) {
			return new CustomObjectData(
				this.ObjectID,
				GetFormattedObjectData(),
				texture,
				Color.White,
				index,
				false,
				type,
				Crafting != null ? new CraftingData(ObjectID, Crafting) : null);
		}

		public string GetFormattedObjectData() {
			return $"{ObjectID}/{Price}/{Edibility}/{ObjectType} {ObjectCategory}/{Name}/{Description}";
		}
	}

	abstract class ModObject : SObject, ICustomObject, ISaveElement, IDrawFromCustomObjectData {
	
		public Texture2D texture;
		public ModObjectData modObjectData;
		public int itemID;

		public abstract CustomObjectData data { get; }

		public ModObject() {

		}

		public ModObject(CustomObjectData data) : base(data.sdvId, 0) {

		}

		public ModObject(CustomObjectData data, int parentSheetIndex) : base(data.sdvId, 0) {

		}

		public Dictionary<string, string> getAdditionalSaveData() {
			return new Dictionary<string, string>() {
				{ "name", this.name }
			};
		}

		public void rebuild(Dictionary<string, string> additionalSaveData, object replacement) {
			name = additionalSaveData["name"];
		}

		public override bool canBeDropped() {
			return true;
		}

		public override bool canBePlacedHere(GameLocation l, Vector2 tile) {
			return false;
		}

		public override bool isPlaceable() {
			return false;
		}

		public override bool canBeShipped() {
			return true;
		}

		public abstract void build();

		public abstract object getReplacement();

		public abstract ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement);
	}
}
