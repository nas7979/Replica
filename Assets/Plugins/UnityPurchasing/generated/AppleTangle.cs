#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class AppleTangle
    {
        private static byte[] data = System.Convert.FromBase64String("4eSy+unj8ePzzDeOoHOR7hkTjGrvzOHm4uLg5ebx+Y+Tk5eU3cjIkOHX6OHksvr05uYY4+LX5ObmGNf6wdfD4eSy4+z0+qaXl4uCx6SClZOTjoGOhIaTgseFnseGiZ7Hl4aVk57HhpSUkoqClMeGhISCl5OGiYSCkJDJhpeXi4LJhIiKyIaXl4uChIa1gouOhomEgseIiceTj46Ux4SClZ3XZeaR1+nh5LL66ObmGOPj5OXmJ4TUkBDd4MuxDD3oxuk9XZT+qFLNYa9hEOrm5uLi59eF1uzX7uHksoBo71PHECxLy8eIl1HY5tdrUKQoPtGYJmCyPkB+XtWlHD8ylnmZRrX4djz5oLcM4gq5nmPKDNFFsKuyC5eLgse1iIiTx6Sm1/nw6tfR19PVx6Sm12Xmxdfq4e7NYa9hEOrm5ubx1/Ph5LLj5PTqppeXi4LHtYiIk9dl41zXZeRER+Tl5uXl5uXX6uHuiYPHhIiJg46TjoiJlMeIgceSlIK+QOLum/Cnsfb5kzRQbMTcoEQyiKKZ+KuMt3GmbiOThez3ZKZg1G1mTESWdaC0siZIyKZUHxwElyoBRKvJp0EQoKqY77nX+OHksvrE4//X8WfzzDeOoHOR7hkTjGrJp0EQoKqYUt1KE+jp53XsVsbxyZMy2+o8hfGD0sTyrPK++lRzEBF7eSi3XSa/t5WGhJOOhILHlJOGk4KKgomTlMnXbP5uOR6sixLgTMXX5Q//2R+37jTRfqvKn1AKa3w7FJB8FZE1kNeoJo6BjoSGk46IicemkpOPiJWOk57WWROUfAk1g+gsnqjTP0XZHp8YjC/i5+Rl5ujn12Xm7eVl5ubnA3ZO7vhiZGL8ftqg0BVOfKdpyzNWd/U/y8eEgpWTjoGOhIaTgseXiIuOhJ6Ypk9/HjYtgXvDjPY3RFwD/M0k+IWLgseUk4aJg4aVg8eTgpWKlMeGcnmd60OgbLwz8dDULCPoqinzjjboetoUzK7P/S8ZKVJe6T65+zEs2tLV1tPX1NG98OrU0tfV197V1tPX1NG914XW7Nfu4eSy4+H05bK01vSXi4LHpIKVk46BjoSGk46IicemkpOPiJWOk57W8dfz4eSy4+T06qaXUPxadKXD9c0g6PpRqnu5hC+sZ/Dvuddl5vbh5LL6x+Nl5u/XZebj1+Ph9OWytNb01/bh5LLj7fTtppeX6uHuzWGvYRDq5ubi4ufkZebm57su/pUSuukymLh8FcLkXbJoqrrqFq4/kXjU84JGkHMuyuXk5ufmRGXmwwUMNlCXOOiiBsAtFoqfCgBS8PDHhomDx4SClZOOgY6EhpOOiInHl087mcXSLcIyPugxjDNFw8T2EEZLx4iBx5OPgseTj4KJx4aXl4uOhIaLgseuiYTJ1sHXw+HksuPs9Pqml9rBgMdt1I0Q6mUoOQxEyB60jbyDyNdmJOHvzOHm4uLg5eXXZlH9ZlTX9uHksuPt9O2ml5eLgseuiYTJ1miUZoch/LzuyHVVH6OvF4ffefIS4Aua3mRstMc03yNWWH2o7YwYzBtW178LvePVa49UaPo5gpQYgLmCW2Xm5+HuzWGvYRCEg+Lm12YV183ht01tMj0DGzfu4NBXkpLG");
        private static int[] order = new int[] { 30,35,29,10,39,40,36,38,28,57,35,21,58,15,36,15,58,30,43,52,25,24,31,32,32,25,46,58,48,32,50,36,49,44,50,42,43,43,38,48,48,48,59,44,49,47,52,58,53,58,54,53,59,56,58,57,57,57,59,59,60 };
        private static int key = 231;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
