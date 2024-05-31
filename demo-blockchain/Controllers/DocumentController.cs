using demo_blockchain.Models;
using demo_blockchain.service;
using demo_blockchain.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace demo_blockchain.Controllers
{
    public class DocumentController : Controller
    {
        private readonly IBlockChainService _blockChainService;
        public DocumentController(
            IBlockChainService blockChainService)
        {
            _blockChainService = blockChainService;
        }
        public IActionResult Upload()
        {
            return View();
        }
        public IActionResult Verify()
        {
            return View();
        }
        public IActionResult GenerateKey()
        {
            return View();
        }


        [HttpGet]
        [Route("Document/GetBlock")]
         public async Task<IActionResult> GetBlock([FromQuery] int? index)
        {
            if (index == null)
            {
                var results = _blockChainService.GetAllBlocks();
                return Ok(results);
            }
            else
            {
                var results = _blockChainService.GetBlockByIndex((int)index);
                return Ok(results);
            }
        }

        [HttpPost]
        [Route("Document/Upload")]
        public async Task<IActionResult> Upload(IFormFile fileToUpload, string pubKeyInput)
        {
            try
            {
                if (fileToUpload == null || fileToUpload.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                byte[] fileBytes;
                using (MemoryStream ms = new MemoryStream())
                {
                    await fileToUpload.CopyToAsync(ms);
                    fileBytes = ms.ToArray();
                }

                Block block = _blockChainService.StoreDocument(fileBytes,pubKeyInput);
                @ViewData["resultHash"] = block.Hash;
                return View();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        [HttpPost]
        [Route("Document/Verify")]
        public async Task<IActionResult> Verify(IFormFile fileToUpload, string hashInput, string privKeyInput)
        {
            try
            {

                if (fileToUpload == null || fileToUpload.Length == 0)
                {
                    @ViewData["result"] = "No file uploaded.";
                }

                byte[] fileBytes;
                using (MemoryStream ms = new MemoryStream())
                {
                    await fileToUpload.CopyToAsync(ms);
                    fileBytes = ms.ToArray();
                }

                if(_blockChainService.VerifyDocument(fileBytes, hashInput, privKeyInput))
                {
                    @ViewData["result"] = "File Verified.";
                }
                else
                {
                    @ViewData["result"] = "File not Verified.";
                }
               
                return View();

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        [HttpPost]
        [Route("Document/GenerateKey")]
        public async Task<IActionResult> GenerateKey(string param = "")
        {
            try
            {
                string publicKey;
                string privateKey;

                RSAUtil.GenerateRSAKeys(out publicKey, out privateKey, 2048);
                @ViewData["pubKey"] = publicKey;
                @ViewData["privKey"] = privateKey;
                return View();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }
    }
}
