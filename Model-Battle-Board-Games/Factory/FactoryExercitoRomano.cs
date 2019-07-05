using BattleBoardGame.Model;
using BattleBoardGame.Model.Factory;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleBoardGame.Model.Factory
{
    class FactoryExercitoRomano : AbstractFactoryExercito
    {
        public override Arqueiro CriarArqueiro()
        {
            return new ArqueiroRomano();
        }

        public override Cavaleiro CriarCavalaria()
        {
            return new CavaleiroRomano();
        }

        public override Guerreiro CriarGuerreiro()
        {
            return new GuerreiroRomano();
        }
    }
}
