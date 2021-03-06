﻿using StardewValley;

namespace MoreTrees.Tools
{
    /// <summary>Represents the bark remover tool.</summary>
    public class BarkRemover : Tool
    {
        /*********
        ** Public Methods
        *********/
        /// <summary>Construct an instance.</summary>
        public BarkRemover() : base(nameof(BarkRemover), 0, 504, 530, false) { }

        /// <summary>Get a new instance of the BarkRemover.</summary>
        /// <returns></returns>
        public override Item getOne() => new BarkRemover();


        /*********
        ** Protected Methods
        *********/
        /// <summary>Get the display name.</summary>
        /// <returns>The display name.</returns>
        protected override string loadDisplayName() => "Bark Remover";

        /// <summary>Get the description.</summary>
        /// <returns>The description.</returns>
        protected override string loadDescription() => "Removes bark from specific trees.";
    }
}
