using System;

namespace Municiple_Project_st10259527.Services.DataStructures
{
    public interface IKey<TK> { int Compare(TK a, TK b); }

    public class ComparableKey<TK> : IKey<TK> where TK : IComparable<TK>
    {
        public int Compare(TK a, TK b) => a.CompareTo(b);
    }

    public class BinarySearch<TK, TV> where TK : IComparable<TK>
    {
        protected readonly IKey<TK> K = new ComparableKey<TK>();
        protected BinaryNode<(TK Key, TV Val)> Root;

        public virtual void Insert(TK key, TV val) { Root = Insert(Root, key, val); }

        public virtual TV Find(TK key)
        {
            var current = Root;
            while (current != null)
            {
                var cmp = K.Compare(key, current.Value.Key);
                if (cmp == 0) return current.Value.Val;
                current = cmp < 0 ? current.Left : current.Right;
            }
            return default;
        }

        protected virtual BinaryNode<(TK, TV)> Insert(BinaryNode<(TK, TV)> node, TK key, TV value)
        {
            if (node == null) return new BinaryNode<(TK, TV)>((key, value));
            var cmp = K.Compare(key, node.Value.Item1);
            if (cmp < 0) node.Left = Insert(node.Left, key, value);
            else if (cmp > 0) node.Right = Insert(node.Right, key, value);
            else node.Value = (key, value);
            return node;
        }
    }

    public class Avl<TK, TV> : BinarySearch<TK, TV> where TK : IComparable<TK>
    {
        int HeightOf(BinaryNode<(TK, TV)> node) => node?.Height ?? 0;
        int BalanceFactor(BinaryNode<(TK, TV)> node) => node == null ? 0 : HeightOf(node.Left) - HeightOf(node.Right);
        void UpdateHeight(BinaryNode<(TK, TV)> node) { if (node != null) node.Height = Math.Max(HeightOf(node.Left), HeightOf(node.Right)) + 1; }

        BinaryNode<(TK, TV)> RotateRight(BinaryNode<(TK, TV)> y)
        { var x = y.Left; var T2 = x.Right; x.Right = y; y.Left = T2; UpdateHeight(y); UpdateHeight(x); return x; }

        BinaryNode<(TK, TV)> RotateLeft(BinaryNode<(TK, TV)> x)
        { var y = x.Right; var T2 = y.Left; y.Left = x; x.Right = T2; UpdateHeight(x); UpdateHeight(y); return y; }

        protected override BinaryNode<(TK, TV)> Insert(BinaryNode<(TK, TV)> node, TK key, TV value)
        {
            node = base.Insert(node, key, value);
            UpdateHeight(node);
            var balance = BalanceFactor(node);
            if (balance > 1 && K.Compare(key, node.Left.Value.Item1) < 0) return RotateRight(node);
            if (balance < -1 && K.Compare(key, node.Right.Value.Item1) > 0) return RotateLeft(node);
            if (balance > 1 && K.Compare(key, node.Left.Value.Item1) > 0) { node.Left = RotateLeft(node.Left); return RotateRight(node); }
            if (balance < -1 && K.Compare(key, node.Right.Value.Item1) < 0) { node.Right = RotateRight(node.Right); return RotateLeft(node); }
            return node;
        }
    }

    public class RedBlack<TK, TV> : BinarySearch<TK, TV> where TK : IComparable<TK>
    {
        bool IsRed(BinaryNode<(TK, TV)> node) => node != null && node.Red;

        BinaryNode<(TK, TV)> RotateLeft(BinaryNode<(TK, TV)> h)
        { var x = h.Right; h.Right = x.Left; x.Left = h; x.Red = h.Red; h.Red = true; return x; }

        BinaryNode<(TK, TV)> RotateRight(BinaryNode<(TK, TV)> h)
        { var x = h.Left; h.Left = x.Right; x.Right = h; x.Red = h.Red; h.Red = true; return x; }

        void FlipColors(BinaryNode<(TK, TV)> h)
        { h.Red = !h.Red; if (h.Left != null) h.Left.Red = !h.Left.Red; if (h.Right != null) h.Right.Red = !h.Right.Red; }

        protected override BinaryNode<(TK, TV)> Insert(BinaryNode<(TK, TV)> node, TK key, TV value)
        {
            if (node == null) { var created = new BinaryNode<(TK, TV)>((key, value)); created.Red = true; return created; }
            var cmp = K.Compare(key, node.Value.Item1);
            if (cmp < 0) node.Left = Insert(node.Left, key, value);
            else if (cmp > 0) node.Right = Insert(node.Right, key, value);
            else node.Value = (key, value);
            if (IsRed(node.Right) && !IsRed(node.Left)) node = RotateLeft(node);
            if (IsRed(node.Left) && IsRed(node.Left.Left)) node = RotateRight(node);
            if (IsRed(node.Left) && IsRed(node.Right)) FlipColors(node);
            return node;
        }

        public override void Insert(TK key, TV val)
        {
            Root = Insert(Root, key, val);
            if (Root != null) Root.Red = false;
        }
    }
}
