using stratumbot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.Core
{
    class Ring
    {
        /// <summary>
        /// Звук при завершении итерации
        /// </summary>
        public static void BabloVoice()
        {
            if (Settings.BabloVoice)
            {
                new System.Media.SoundPlayer("Common/bablo.wav").Play();
            }
        }
    }
}
