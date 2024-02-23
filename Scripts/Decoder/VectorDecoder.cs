using ScenarioFlow;
using ScenarioFlow.Scripts.SFText;
using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SavableSFSample
{
    public class VectorDecoder : IReflectable
    {
        private readonly Regex Vec3Regex = new Regex(@"^\((.*?),(.*?),(.*?)\)$");

		[DecoderMethod]
		[Description("A decoder for the 'Vector3' type.")]
		[Description("e.g. '(1, 2, 3)', '(-1, -2, -3)'")]
		public Vector3 ConvertToVector3(string input)
        {
            var vec3Match = Vec3Regex.Match(input);
            if (!vec3Match.Success)
            {
                throw new ArgumentException($"Failed to convert '{input}' to a value of the Vector3 type.");
            }
            var x = float.Parse(vec3Match.Groups[1].Value);
            var y = float.Parse(vec3Match.Groups[2].Value);
            var z = float.Parse(vec3Match.Groups[3].Value);
            return new Vector3(x, y, z);
        }
    }
}
