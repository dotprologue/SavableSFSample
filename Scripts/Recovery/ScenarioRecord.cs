using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SavableSFSample
{
    [Serializable]
    public class ScenarioRecord
    {
        [SerializeField]
        private Fragment[] fragments;

        public IEnumerable<Fragment> Fragments => fragments;

        public ScenarioRecord(IEnumerable<Fragment> fragments)
        {
            if (fragments == null)
                throw new ArgumentNullException(nameof(fragments));

            this.fragments = fragments.ToArray();
        }

        [Serializable]
        public class Fragment
        {
            [SerializeField]
            private string id;
            [SerializeField]
            private string value;

            public string Id => id;
            public string Value => value;

            public Fragment(string id, string value)
            {
                this.id = id;
                this.value = value;
            }
        }
    }
}
