// GameObject class (GameObject.cs)
namespace PicoPark
{
    public abstract class GameObject
    {
        public string Id { get; private set; }
        public Vector2D Position { get; set; }

        protected GameObject(string id, Vector2D position)
        {
            Id = id;
            Position = position;
        }

        protected GameObject(string id) : this(id, new Vector2D())
        {
        }
    }
}
