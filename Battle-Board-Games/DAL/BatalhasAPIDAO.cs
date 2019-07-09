using BattleBoardGame.Model;
using BattleBoardGame.Model.DAL;
using BattleBoardGames.Areas.Identity.Data;
using BattleBoardGames.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BattleBoardGames.DAL
{
    public class BatalhasAPIDAO
    {
        private readonly ModelJogosDeGuerra _context;
        UserManager<BattleBoardGamesUser> _userManager;

        public BatalhasAPIDAO(ModelJogosDeGuerra context, UserManager<BattleBoardGamesUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Task<int> retornarQuantidadeBatalhas()
        {
            return _context.Batalhas.CountAsync();
        }

        public IEnumerable<Batalha> retornarTodasBatalhas()
        {
            return _context.Batalhas.ToList();
        }

        public IEnumerable<Batalha> retornarBatalhasFinalizadas()
        {
            return _context.Batalhas.Where(b => b.Vencedor != null).ToList();
        }

        public int retornarBatalhasJogador(string name)
        {
            return _context.Batalhas
                .Where(b => (b.ExercitoBranco != null &&
                            b.ExercitoBranco.UsuarioId ==
                            name)
                            ||
                            (b.ExercitoPreto != null &&
                            b.ExercitoPreto.UsuarioId ==
                            name))
                            .Count();
        }

        public Exercito buscarExercitoPorID(int ExercitoId)
        {
            return _context.Exercitos.Where(e => e.Id == ExercitoId).FirstOrDefault();
        }

        public Task AlterarDadosAsync()
        {
            return _context.SaveChangesAsync();
        }

        public Task<Batalha> buscarBatalhaPorID(int id)
        {
            return _context.Batalhas.Include(b => b.ExercitoPreto)
                .Include(b => b.ExercitoPreto.Usuario)
                .Include(b => b.ExercitoBranco)
                .Include(b => b.ExercitoBranco.Usuario)
                .Include(b => b.Tabuleiro)
                .Include(b => b.Tabuleiro.ElementosDoExercito)
                .Include(b => b.Turno)
                .Include(b => b.Turno.Usuario).Where(b => b.Id == id).FirstOrDefaultAsync();
        }

        public Batalha retornarBatalhaPorUsuario(Usuario usuario, int id)
        {
            return _context.Batalhas
                .Include(b => b.ExercitoPreto)
                .Include(b => b.ExercitoBranco)
                .Include(b => b.Tabuleiro)
                .Include(b => b.Turno)
                .Include(b => b.Turno.Usuario)
                .Where(b =>
                (b.ExercitoBranco.Usuario.Id == usuario.Id
                || b.ExercitoPreto.Usuario.Id == usuario.Id)
                && (b.ExercitoBranco != null && b.ExercitoPreto != null)
                && b.Id == id).FirstOrDefault();
        }

        public void AlterarDados()
        {
            _context.SaveChanges();
        }

        public ElementoDoExercito buscarElementoPorId(int ElementoId)
        {
            return _context.ElementosDoExercitos
                 .Include(el => el.Exercito)
                     .FirstOrDefault(el => el.Id == ElementoId);
        }

        public Batalha retornarBatalhaPorId(int batalhaId)
        {
            return _context.Batalhas
                 .Include(b => b.Tabuleiro)
                 .Include(b => b.Tabuleiro.ElementosDoExercito)
                 .Include(b => b.ExercitoBranco)
                 .Include(b => b.ExercitoPreto)
                 .Include(b => b.Turno)
                 .Include(b => b.Vencedor)
                 .Include(b => b.ExercitoBranco.Usuario)
                 .Include(b => b.ExercitoPreto.Usuario)
                 .FirstOrDefault(
                     m => m.Id == batalhaId);
        }

        public void AdicionarBatalha(Batalha batalha)
        {
            _context.Batalhas.Add(batalha);
        }

        public Batalha buscarBatalhaPendente(Usuario usuario)
        {
            return _context.Batalhas.Include(b => b.ExercitoBranco)
                .Include(b => b.ExercitoPreto)
                .FirstOrDefault(b =>
            (b.ExercitoBrancoId == null
            || b.ExercitoPretoId == null) &&
            (b.ExercitoBranco.UsuarioId != usuario.Id
            && b.ExercitoPreto.UsuarioId != usuario.Id));
        }

        public Task<Batalha> buscarBatalha(int id)
        {
            return _context.Batalhas.FindAsync(id);
        }

        public void removerBatalha(Batalha batalha)
        {
            _context.Batalhas.Remove(batalha);
        }

        public void AlterarBatalha(Batalha batalha)
        {
            _context.Entry(batalha).State = EntityState.Modified;
        }
    }
}
