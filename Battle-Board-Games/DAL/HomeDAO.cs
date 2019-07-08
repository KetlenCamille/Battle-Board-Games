using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BattleBoardGame.Model;
using BattleBoardGame.Model.DAL;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BattleBoardGames.DAL
{
    public class HomeDAO
    {
        private readonly ModelJogosDeGuerra _context;

        public HomeDAO(ModelJogosDeGuerra context)
        {
           _context = context;
        }

        public Batalha retornarBatalha(int batalhaId)
        {
            return _context.Batalhas
                   .Where(b => b.Id == batalhaId).FirstOrDefault();
        }
    }
}
