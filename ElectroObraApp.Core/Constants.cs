using System;

namespace ElectroObraApp.Core;

public static class Constants
{
    public static class TiposMovimiento
    {
        public static readonly Guid Ingreso = Guid.Parse("00000000-0000-0000-0000-000000000001");
        public static readonly Guid Gasto = Guid.Parse("00000000-0000-0000-0000-000000000002");
        public static readonly Guid Adelanto = Guid.Parse("00000000-0000-0000-0000-000000000003");
        public static readonly Guid Ajuste = Guid.Parse("00000000-0000-0000-0000-000000000004");
    }
}
