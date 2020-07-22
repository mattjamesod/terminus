﻿using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
    public class Party : List<PartyMember>
    {
        public bool bAllFollowing = false;

        private int formationPos = -1;
        private int FormationPos
        {
            get => formationPos;
            set => formationPos = value > 3 ? 3 : value;
        }

        public void Add(GameObject newMember)
           => this.Add(new PartyMember(newMember));

        public void AddRange(IEnumerable<GameObject> newMembers)
            => this.AddRange(newMembers);

        public bool Contains(GameObject predMember)
            => this.Any(x => x.GameObject == predMember);

        public void FormAround(PartyMember focus, float radius)
        {
            if (!this.Contains(focus))
                return;

            var playerDirection = focus.GameObject.transform.forward;
            var orthPlayerDirection = Vector3.Cross(playerDirection, Vector3.up).normalized;

            var alpha = radius * playerDirection;
            var beta = radius * orthPlayerDirection;

            this.Where(x => x.bFollowing).ToList()
                .ForEach(x => x.Ai.MoveTo(GetNextFormationPos(focus.GameObject.transform.position, alpha, beta)));
            formationPos = -1;
        }

        private Vector3 GetNextFormationPos(Vector3 position, Vector3 a, Vector3 b)
        {
            switch (++formationPos)
            {
                case 0: return position - a + b;
                case 1: return position - a - b;
                case 2: return position - 2 * a;
                default: return new Vector3(50, 0, 50);
            }
        }
    }

    public class PartyMember
    {
        public CharacterBehaviour Ai { get; set; }

        public PartyMember(GameObject GameObject, Vector3 formationPos = new Vector3(), bool bFollowing = false)
        {
            this.GameObject = GameObject;
            this.bFollowing = bFollowing;
            this.FormationPos = formationPos;

            Ai = GameObject.GetComponent<CharacterBehaviour>();
        }

        public GameObject GameObject { get; set; }
        public bool bFollowing { get; set; }
        public Vector3 FormationPos { get; set; }

        private Action formationIdle;

        public void StartFollowing(Transform leader)
        {


            formationIdle = () => Ai.MoveTo(FormationPos);

            Ai.onFollowingIdle.Add(formationIdle);
            Ai.StartFollowing(leader);
        }

        public void StopFollowing()
        {
            Ai.StopFollowing();
            Ai.onFollowingIdle.Clear();
        }
    }
}
