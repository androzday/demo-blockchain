using demo_blockchain.Models;

namespace demo_blockchain.service
{
    public interface IBlockChainService
    {
        public Block StoreDocument(byte[] data, string pubKey);
        public bool VerifyDocument(byte[] data, string hash, string privKey);
        public Block AddBlock(Block block);
        public Block GetBlockByIndex(int index);
        public List<Block> GetAllBlocks();
        public Block GetBlockByHash(string hash);
    }
}
