using demo_blockchain.Models;
using demo_blockchain.Utils;
using Newtonsoft.Json;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace demo_blockchain.service
{
    public class BlockChainService : IBlockChainService
    {
        private int difficulty = 2;
        private string filePath = Path.Combine(Directory.GetCurrentDirectory(), "pub_ledger.json");
        public Block StoreDocument(byte[] data, string pubKey)
        {
            try
            {
                if (!File.Exists(filePath) && Ledger.Blocks.Count == 0)
                {
                    this.UpdateLedger();
                }
                string md5Data = this.ConvertToMd5(data);
                byte[] encryptedDataByte = RSAUtil.Encrypt(Encoding.UTF8.GetBytes(md5Data), pubKey);
                
                Block block = new Block(Convert.ToBase64String(encryptedDataByte));
                block = this.AddBlock(block);
                this.UpdateLedger();
                return block;
            }catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public bool VerifyDocument(byte[] data, string hash, string privKey)
        {

            try
            {
                if(Ledger.Blocks.Count == 0)
                {
                    this.GetLedger();
                }
                Block block = this.GetBlockByHash(hash);
                if (block == null)
                {
                    throw new ArgumentNullException("block empty");
                }

                string encryptedData = block.Data;
                byte[] decryptedDataByte = RSAUtil.Decrypt(Convert.FromBase64String(encryptedData), privKey);
                string md5Decrypted = Encoding.UTF8.GetString(decryptedDataByte);
                string md5 = this.ConvertToMd5(data);
                Console.WriteLine("md5 == md5decrypt");
                Console.WriteLine($"{md5} == {md5Decrypted}");
                if (md5 != md5Decrypted)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Block AddBlock(Block block)
        {

            if (this.IsValid())
            {
                Block latestBlock = Ledger.Blocks.LastOrDefault();

                if (latestBlock == null)
                {
                    latestBlock = new Block("");
                    Ledger.Blocks.Add(latestBlock);
                }

                block.Index = latestBlock.Index + 1;
                block.PrevHash = latestBlock.Hash;
                block.Mine(this.difficulty);

                Ledger.Blocks.Add(block);
                return block;
            }
            return null;
            
        }

        public Block GetBlockByHash(string hash)
        {
            return Ledger.Blocks.FirstOrDefault(block => block.Hash == hash);

        }
        public Block GetBlockByIndex(int index)
        {
            return Ledger.Blocks[index];
        }
        public List<Block> GetAllBlocks()
        {
            return Ledger.Blocks;
        }
      
        private bool IsValid(string blockHash = "")
        {
            // check all blocks
            int iNumberBlocks = Ledger.Blocks.Count;

            // check all blocks until the given, optional hash
            if (blockHash != "")
            {
                Block block = Ledger.Blocks.FirstOrDefault(h => h.Hash == blockHash);
                if (block != null)
                {
                    iNumberBlocks = block.Index + 1;
                }
            }

            // loop through all blocks, generate their hashes and compare

            for (int i = 1; i < iNumberBlocks; i++)
            {
                Block currentBlock = Ledger.Blocks[i];
                Block previousBlock = Ledger.Blocks[i - 1];
                string currentHash = currentBlock.GenerateBlockHash();
                if (currentBlock.Hash != currentHash)
                {
                    return false;
                }

                if (currentBlock.PrevHash != previousBlock.Hash)
                {
                    return false;
                }
            }
            return true;
        }
        private string ConvertToMd5(byte[] data)
        {
            string result = "";
            using (var md5 = MD5.Create())
            {

                result = BitConverter.ToString(md5.ComputeHash(data)).Replace("-", "").ToLower();
            }
            return result;
        }
        private byte[] ConvertToMd5Byte(byte[] data)
        {
            byte[] result;
            using (var md5 = MD5.Create())
            {

                result = md5.ComputeHash(data);
            }
            return result;
        }
        private List<Block> GetLedger()
        {
            string json = File.ReadAllText(filePath);

            Ledger.Blocks.AddRange(JsonConvert.DeserializeObject<List<Block>>(json));
            return Ledger.Blocks;
        }
        private void UpdateLedger()
        {
            string json = JsonConvert.SerializeObject(Ledger.Blocks.ToList(), Formatting.Indented);
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, json);
                Console.WriteLine("File created successfully.");
            }
            else
            {
                File.Delete(filePath);
                File.WriteAllText(filePath, json);
                Console.WriteLine("File updated successfully.");
            }
        }
        
    }
}
