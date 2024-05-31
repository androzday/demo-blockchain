using Microsoft.AspNetCore.Authentication;
using System.Security.Cryptography;
using System.Text;

namespace demo_blockchain.Models
{
    public class Block
    {
        public string Hash { get; set; }
        public string PrevHash { get; set; }
        public int Nonce { get; set; }
        public DateTime Timestamp { get; set; }
        public string Data { get; }
        public int Index { get; set; }

        public Block(string data)
        {
            this.Index = 0;
            this.Data = data ?? throw new ArgumentNullException(nameof(data));
            this.Nonce = 0;
            this.PrevHash = "";
            this.Timestamp = DateTime.UtcNow;
            this.Hash = GenerateBlockHash();
        }

        public string GenerateBlockHash()
        {
            SHA256 sha256 = SHA256.Create();

            byte[] bInput = Encoding.ASCII.GetBytes($"{Timestamp}-{PrevHash ?? ""}-{Data}-{Nonce}");
            byte[] bOutput = sha256.ComputeHash(bInput);

            return Base64UrlTextEncoder.Encode(bOutput);
        }

        public void Mine(int difficulty)
        {
            string sLeadingZeros = new string('0', difficulty);

            while (this.Hash == null || this.Hash.Substring(0, difficulty) != sLeadingZeros)
            {
                this.Nonce++;
                this.Hash = this.GenerateBlockHash();
            }
        }
    }

   
}
