namespace StratisRpc
{
    public static class TestData
    {
        public static string[] txIds = {
            "69130e612c49b3da86859c10387c309e6949038e88c1e6cb4ab149a458e3fadc",
            "88be2a3925178361352f97b2269d89554f7bf8e03f8ddbda3ae8a51f99ebc188",
            "5ce0b9f2b231469ee50152e61b312bab16c87242baa96233f75783cb123dfb1e",
            "ebcb307ad08a621a65277ab8de81397dbdec900b5487955f7caa10b16c2a6400",
            "7fd7976841355b32b952aac2da00c6128b17fb0e0a15f3e4b6df895a91bf2169",
            "aa5db4227e547e43d9a23f3bc08442086f3e961b1824836f9fb690c3af82d1ac",
            "3fa93fe1dfaef4620f84ee9219fe5ec09698449393326d46b7c02106d688ccba",
            "d32bf769b89cd555715e4ec94f86362004a5691cd602110210895d58cbac98ec",
            "4e20b550fd5231bb325d509cea4832f2d2613cc95eb13171e22e11586095d7f9",
            "dcaaf40c6d5b44f34e566433783fe5299532a308e5adfcfb3551505d761add4e"
        };

        public static string GetTxId(int index = 0)
        {
            return txIds[index % txIds.Length];
        }
    }
}
