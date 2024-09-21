using Nethereum.HdWallet;

class MnemonicValidator
{
    public static void ValidateMnemonic()
    {
        var mnemonic = " user mnomic here =>PlayerPrefs.GetString(RdUserPrefKeys.MY_12_WORDS)";

        var wallet = new Wallet(mnemonic, null);
        
        if (!wallet.IsMnemonicValidChecksum)
        {
            throw new Exception("Invalid mnemonic checksum.");
        }
    }
}