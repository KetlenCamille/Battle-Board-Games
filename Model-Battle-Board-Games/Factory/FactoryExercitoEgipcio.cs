using BattleBoardGame.Model;
using BattleBoardGame.Model.Factory;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelBattleBoardGames.Factory
{
    class FactoryExercitoEgipcio : AbstractFactoryExercito
    {
        public override Arqueiro CriarArqueiro()
        {
            return new ArqueiroEgipicio();
        }

        public override Cavaleiro CriarCavalaria()
        {
            return new CavaleiroEgipicia();
        }

        public override Guerreiro CriarGuerreiro()
        {
            return new GuerreiroEgipicio();
        }
    }
}
