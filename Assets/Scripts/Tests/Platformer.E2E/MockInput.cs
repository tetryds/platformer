using Platformer.Gameplay;

namespace Tests.Platformer.E2E
{
    public class MockInput : APlayerInput
    {
        public new float Horizontal { set => base.Horizontal = value; }
        public new bool Jump { set => base.Jump = value; }
        public new bool Dash { set => base.Dash = value; }
    }
}
