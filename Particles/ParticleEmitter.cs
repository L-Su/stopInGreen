using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace stopInGreen.Particles
{
    public class ParticleEmitter
    {

        private Dictionary<int, Particle> m_particles = new Dictionary<int, Particle>();
        private Texture2D m_textGlow;
        private Texture2D m_texFire;
        private MyRandom m_random = new MyRandom();

        private TimeSpan m_rate;
        private int m_sourceX;
        private int m_sourceY;
        private int m_particleSize;
        private int m_speed;
        private TimeSpan m_lifetime;
        private TimeSpan m_switchover;

        public Vector2 Gravity { get; set; }

        public ParticleEmitter(ContentManager content, TimeSpan rate, int sourceX, int sourceY, int size, int speed, TimeSpan lifetime)
        {
            m_rate = rate;
            m_sourceX = sourceX;
            m_sourceY = sourceY;
            m_particleSize = size;
            m_speed = speed;
            m_lifetime = lifetime;
            //m_switchover = wwitchover;

            m_textGlow = content.Load<Texture2D>("Images/gold_glow_256");
            //m_texFire = content.Load<Texture2D>("Images/fire");

            this.Gravity = new Vector2(0, 0);
        }

        private TimeSpan m_accumulated = TimeSpan.Zero;

        /// <summary>
        /// Generates new particles, updates the state of existing ones and retires expired particles.
        /// </summary>
        public void update(GameTime gameTime)
        {


            
            //
            // Generate particles at the specified rate
            m_accumulated += gameTime.ElapsedGameTime;
            while (m_accumulated > m_rate)
            {
                m_accumulated -= m_rate;


                Particle p = new Particle(
                m_random.Next(),
                new Vector2(m_sourceX, m_sourceY),
                m_random.nextCircleVector(),
                (float)m_random.nextGaussian(m_speed, 1),
                m_lifetime);

                if (!m_particles.ContainsKey(p.name))
                {
                    m_particles.Add(p.name, p);
                }
                
                
                
                
            }


            //
            // For any existing particles, update them, if we find ones that have expired, add them
            // to the remove list.
            List<int> removeMe = new List<int>();
            foreach (Particle p in m_particles.Values)
            {
                p.lifetime -= gameTime.ElapsedGameTime;
                if (p.lifetime < TimeSpan.Zero)
                {
                    //
                    // Add to the remove list
                    removeMe.Add(p.name);
                }
                //
                // Update its position
                p.position += (p.direction * p.speed);
                //
                // Have it rotate proportional to its speed
                p.rotation += p.speed / 50.0f;
                //
                // Apply some gravity
                p.direction += this.Gravity;
            }

            //
            // Remove any expired particles
            foreach (int Key in removeMe)
            {
                m_particles.Remove(Key);
            }
        }

        /// <summary>
        /// Renders the active particles
        /// </summary>
        public void draw(SpriteBatch spriteBatch)
        {
            Rectangle r = new Rectangle(0, 0, m_particleSize, m_particleSize);
            foreach (Particle p in m_particles.Values)
            {
/*                Texture2D texDraw;
                if (p.lifetime < m_switchover)
                {
                    texDraw = m_texSmoke;
                }
                else
                {
                    texDraw = m_texFire;
                }
*/
                r.X = (int)p.position.X;
                r.Y = (int)p.position.Y;
                spriteBatch.Draw(
                    m_textGlow,
                    r,
                    null,
                    Color.White,
                    p.rotation,
                    new Vector2(m_textGlow.Width / 2, m_textGlow.Height / 2),
                    SpriteEffects.None,
                    0);
            }
        }
    }
}
