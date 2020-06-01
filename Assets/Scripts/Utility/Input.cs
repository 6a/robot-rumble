using UnityEngine;
using System.Collections.Generic;

namespace RR.Utility.Input
{
    public enum RRAction
    {
        Jump,
        Attack,
        Block,
        AnimationCancel
    }

    public enum RRUICommand
    {
        Exit,
        Submit,
        NavigateBack,
        NavigateForward
    }

    public class InputFrame
    {
        public Queue<RRAction> Keys { get; set; }
        public Vector2 Look { get; set; }
        public Vector2 Move { get; set; }
        public int Scroll { get; set; }

        public InputFrame()
        {
            Keys = new Queue<RRAction>();
        }

        public override string ToString()
        {
            var q = "";

            if (Keys != null)
            {
                foreach (var k in Keys)
                {
                    q += k.ToString() + ", ";
                }

                q.Trim(", ".ToCharArray());
            }

            return $"Keys: [ {q} ]\nMouseDelta: {Look} | MoveDelta: {Move} | ScrollDelta: {Scroll}";
        }
    }
}