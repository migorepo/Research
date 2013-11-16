using System;

namespace Wp8.Framework.Utils.Utils
{
    public static class IdentityGenerator
    {
        public static int Generate(Predicate<int> isAlreadyInUsePredicate)
        {
            var rand = new Random((int)DateTime.Now.Ticks % int.MaxValue);
            while (true)
            {
                var result = rand.Next();
                if (!isAlreadyInUsePredicate(result))
                    return result;
            }
        }
    }
}

