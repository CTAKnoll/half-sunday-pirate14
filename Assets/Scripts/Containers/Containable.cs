using UI.Model;
using UnityEngine;

namespace UI.Containers
{
    // TODO: WTF is generic covariance
    public interface Containable<T, out TServedUI> where T: Containable<T, TServedUI> 
                                            where TServedUI: IUIController
    {
        static T Empty { get; }
        TServedUI Serve(Transform position);
    }
}