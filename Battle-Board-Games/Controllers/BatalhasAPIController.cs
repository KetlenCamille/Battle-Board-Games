﻿using BattleBoardGame.Model;
using BattleBoardGame.Model.DAL;
using BattleBoardGames.Areas.Identity.Data;
using BattleBoardGames.DAL;
using BattleBoardGames.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BattleBoardGame.Model.Factory.AbstractFactoryExercito;

namespace Battle_Board_Games.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BatalhasAPIController : ControllerBase
    {
        BatalhasAPIDAO BatalhasAPIDAO;
        UsuarioService _usuarioService;

        public BatalhasAPIController (ModelJogosDeGuerra context, UserManager<BattleBoardGamesUser> userManager)
        {
            BatalhasAPIDAO = new BatalhasAPIDAO(context, userManager);
            _usuarioService = new UsuarioService(context, userManager);
        }

        [HttpGet]
        [Route("QtdBatalhas")]
        public async Task<IActionResult> ObterQuantidadeBatalhas()
        {
            return Ok(await BatalhasAPIDAO.retornarQuantidadeBatalhas());
        }

        // GET: api/BatalhasAPI
        [Authorize]
        [HttpGet]
        public IEnumerable<Batalha> GetBatalhas(bool Finalizada = false)
        {
            IEnumerable<Batalha> batalhas;
            if (Finalizada)
            {
                batalhas = BatalhasAPIDAO.retornarBatalhasFinalizadas();
            }
            else
            {
                batalhas = BatalhasAPIDAO.retornarTodasBatalhas();
            }
            return batalhas;
        }

        [Authorize]
        [HttpGet]
        [Route("QtdBatalhasJogador")]
        public async Task<IActionResult> GetBatalhasJogador()
        {
            int batalhas = BatalhasAPIDAO.retornarBatalhasJogador(User.Identity.Name);
            return Ok(batalhas);
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EscolherNacao(Nacao nacao, int ExercitoId)
        {

            Exercito exercito = BatalhasAPIDAO.buscarExercitoPorID(ExercitoId);
            exercito.Nacao = nacao;

            await BatalhasAPIDAO.AlterarDadosAsync();
               
            return Ok(exercito);
        }

        // GET: api/BatalhasAPI?id=5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetBatalha([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Batalha batalha = await BatalhasAPIDAO.buscarBatalhaPorID(id);

            if (batalha == null)
            {
                return NotFound();
            }

            return Ok(batalha);
        }

        [Route("IniciarBatalha/{id}")]
        [Authorize]
        public async Task<IActionResult> IniciarBatalha(int id)
        {
            Usuario usuario = _usuarioService.ObterUsuarioEmail(this.User);


            //Get batalha
            Batalha batalha = BatalhasAPIDAO.retornarBatalhaPorUsuario(usuario, id);

            if (batalha == null)
            {
                return NotFound();
            }

            if (batalha.Tabuleiro == null)
            {
                batalha.Tabuleiro = new Tabuleiro();
                batalha.Tabuleiro.Altura = 8;
                batalha.Tabuleiro.Largura = 8;
            }
            try
            {
                if (batalha.Estado == Batalha.EstadoBatalhaEnum.NaoIniciado)
                {
                    batalha.Tabuleiro.IniciarJogo(batalha.ExercitoBranco, batalha.ExercitoPreto);
                    Random r = new Random();
                    batalha.Turno = r.Next(100) < 50
                        ? batalha.ExercitoPreto :
                        batalha.ExercitoBranco;
                    batalha.Estado = Batalha.EstadoBatalhaEnum.Iniciado;
                }
            }
            catch (ArgumentException arg)
            {
                BadRequest("Não foi escolhido uma nação.");
            }

            BatalhasAPIDAO.AlterarDados();
            
            return Ok(batalha);
        }

        [Authorize]
        [Route("Jogar")]
        [HttpPost]
        public async Task<IActionResult> Jogar([FromBody]Movimento movimento)
        {
            movimento.Elemento = BatalhasAPIDAO.buscarElementoPorId(movimento.ElementoId);

            if (movimento.Elemento == null)
            {
                return NotFound();
            }

            movimento.Batalha = BatalhasAPIDAO.retornarBatalhaPorId(movimento.BatalhaId);

            var usuario = this._usuarioService.ObterUsuarioEmail(this.User);


            if (usuario.Id != movimento.AutorId)
            {
                return Forbid("O usuário autenticado não é o autor da jogada");
            }

            var batalha = movimento.Batalha;
            if (movimento.AutorId != movimento.Elemento.Exercito.UsuarioId)
            {
                //Usuário não é o dono do exercito.
                return Forbid("O jogador não é dono do exercito");
            }
            if (movimento.AutorId == batalha.Turno.UsuarioId)
            {
                if (!batalha.Jogar(movimento))
                {
                    return BadRequest("A jogada é invalida");
                }
                batalha.Turno = null;
                batalha.TurnoId = batalha.TurnoId == batalha.ExercitoBrancoId ?
                    batalha.ExercitoPretoId : batalha.ExercitoBrancoId;
                await BatalhasAPIDAO.AlterarDadosAsync();
                return Ok(batalha);
            }
            return BadRequest("Operação não realizada");

        }

        // PUT: api/BatalhasAPI/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBatalha([FromRoute] int id, [FromBody] Batalha batalha)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != batalha.Id)
            {
                return BadRequest();
            }

            BatalhasAPIDAO.AlterarBatalha(batalha);
            

            try
            {
                await BatalhasAPIDAO.AlterarDadosAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (BatalhasAPIDAO.buscarBatalha(id) == null)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/BatalhasAPI
        [HttpPost]
        public async Task<IActionResult> PostBatalha([FromBody] Batalha batalha)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            BatalhasAPIDAO.AdicionarBatalha(batalha);
            await BatalhasAPIDAO.AlterarDadosAsync();

            return CreatedAtAction("GetBatalha", new { id = batalha.Id }, batalha);
        }

        [HttpGet]
        [Route("CriarBatalha/{idNacao}")]
        [Authorize]
        public async Task<IActionResult> CriarBatalha(int idNacao)
        {
            Usuario usuario = _usuarioService.ObterUsuarioEmail(this.User);

            Batalha batalha = BatalhasAPIDAO.buscarBatalhaPendente(usuario);

            if (batalha == null)
            {
                batalha = new Batalha();
                BatalhasAPIDAO.AdicionarBatalha(batalha);
            }

            Exercito e = new Exercito();
            e.Usuario = usuario;

            switch (idNacao)
            {
                case 1:
                    e.Nacao = Nacao.India;
                    break;
                case 2:
                    e.Nacao = Nacao.Persia;
                    break;
                case 3:
                    e.Nacao = Nacao.Egito;
                    break;
                case 4:
                    e.Nacao = Nacao.Romano;
                    break;
                default:
                    break;
            }


            if (batalha.ExercitoBrancoId == null)
            {
                batalha.ExercitoBranco = e;
            }
            else if (batalha.ExercitoPretoId == null)
            {
                batalha.ExercitoPreto = e;
            }
            BatalhasAPIDAO.AlterarDados();
            return Ok(batalha);
        }



        // DELETE: api/BatalhasAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBatalha([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Batalha batalha = await BatalhasAPIDAO.buscarBatalha(id);
                
            if (batalha == null)
            {
                return NotFound();
            }

            BatalhasAPIDAO.removerBatalha(batalha);

            await BatalhasAPIDAO.AlterarDadosAsync();

            return Ok(batalha);
        }

    }
}