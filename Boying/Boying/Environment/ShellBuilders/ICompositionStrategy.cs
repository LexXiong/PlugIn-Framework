using Boying.Environment.Configuration;
using Boying.Environment.Descriptor.Models;
using Boying.Environment.ShellBuilders.Models;

namespace Boying.Environment.ShellBuilders
{
    /// <summary>
    /// Service at the host level to transform the cachable descriptor into the loadable blueprint.
    /// </summary>
    public interface ICompositionStrategy
    {
        /// <summary>
        /// Using information from the IExtensionManager, transforms and populates all of the
        /// blueprint model the shell builders will need to correctly initialize a tenant IoC container.
        /// </summary>
        ShellBlueprint Compose(ShellSettings settings, ShellDescriptor descriptor);
    }
}