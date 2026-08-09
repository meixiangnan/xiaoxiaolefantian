using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{

    public class KeyTileEffect : TileEffect
    {
        [SerializeField] Particle hitParticle;


        private int stage;

        public override void Initialise()
        {
            ParticlesController.RegisterParticle(hitParticle);
            
            DockBehavior.MatchCombined += OnMatchCombined;
        }

        public override void OnCreated(TileBehavior tileBehavior)
        {
            if (null != tileBehavior.KeySpriteRenderer)
            {
                tileBehavior.KeySpriteRenderer.gameObject.SetActive(true);
            }
        }

        private void OnMatchCombined(List<ISlotable> slotables)
        {
            var first = slotables[0];
            foreach (var islot in slotables)
            {
                if (islot is TileBehavior tile && tile.Effect != null && tile.Effect.GetType() == typeof(KeyTileEffect))
                {
                    LevelController.instance.OnMatchCompleted(true);
                    tile.KeySpriteRenderer.gameObject.SetActive(false);
                    return;
                }
            }
        }

        public override void OnTileSubmitted() 
        {
            
            
        }

        public override void OnDisabled(TileBehavior tileBehavior)
        {
        }

       
        public override void Clear()
        {

            Destroy(gameObject);
            DockBehavior.MatchCombined -= OnMatchCombined;
        }

        public override bool IsClickAllowed()
        {
            return true;
        }
    }
}
