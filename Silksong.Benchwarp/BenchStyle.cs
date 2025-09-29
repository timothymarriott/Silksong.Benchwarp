﻿using System.Collections.Generic;
using System.Linq;
using Modding.Converters;
using Newtonsoft.Json;
using UnityEngine;

namespace Benchwarp
{
    public class BenchStyle
    {
        public static readonly Dictionary<string, BenchStyle> _styles;
        private static readonly HashSet<string> _validStyles;

        public static BenchStyle GetStyle(string style)
        {
            return style != null ? _styles.TryGetValue(style, out BenchStyle bs) ? bs : null : null;
        }

        /// <summary>
        /// Returns true if both the style exists and its sprites exist.
        /// </summary>
        public static bool IsValidStyle(string style) => _validStyles.Contains(style);

        public static IEnumerable<string> StyleNames => _styles.Keys;

        public BenchStyle() { }
        public string style;
        [JsonConverter(typeof(Vector3Converter))] public Vector3 offset;
        public bool distinctLitSprite;

        [JsonConverter(typeof(Vector2Converter))] public Vector2 triggerSize;
        [JsonConverter(typeof(Vector2Converter))] public Vector2 triggerOffset;

        [JsonConverter(typeof(Vector3Converter))] public Vector3 localScale;
        [JsonConverter(typeof(Vector3Converter))] public Vector3 litOffset;
        public string spriteName;

        // fsm parameters
        public bool tilter;
        public float tiltAmount;
        [JsonConverter(typeof(Vector3Converter))] public Vector3 adjustVector;

        public void ApplyFsmAndPositionChanges(GameObject bench, Vector3 position)
        {
            bench.transform.position = position + offset;
            bench.transform.localScale = new Vector3(localScale.x, localScale.y, 1f);
            bench.transform.Find("Lit").localPosition = litOffset;

            if (tilter)
            {
                bench.transform.SetRotation2D(tiltAmount);
            }
            else
            {
                bench.transform.SetRotation2D(0);
            }

            PlayMakerFSM fsm = bench.LocateMyFSM("Bench Control");
            HutongGames.PlayMaker.FsmVariables fv = fsm.FsmVariables;
            fv.FindFsmBool("Tilter").Value = tilter;
            fv.FindFsmFloat("Tilt Amount").Value = tiltAmount;
            fv.FindFsmVector3("Adjust Vector").Value = adjustVector;

            BoxCollider2D box = bench.GetComponent<BoxCollider2D>();
            box.size = triggerSize;
            box.offset = triggerOffset;
        }

        public void ApplyDefaultSprite(GameObject bench)
        {
            bench.GetComponent<SpriteRenderer>().sprite = SpriteManager.GetSprite(spriteName);
        }

        public void ApplyLitSprite(GameObject bench)
        {
            bench.transform.Find("Lit").GetComponent<SpriteRenderer>().sprite = SpriteManager.GetSprite(distinctLitSprite ? spriteName + "_lit" : spriteName);
        }

        static BenchStyle()
        {
            _styles = JsonUtil.Deserialize<Dictionary<string, BenchStyle>>("Silksong.Benchwarp.Resources.styles.json");
            _validStyles = SpriteManager.GetValidStyles(StyleNames);
            foreach (string s in StyleNames.Except(_validStyles)) Benchwarp.log.LogWarning($"Invalid style: {s}");
        }
    }
}
