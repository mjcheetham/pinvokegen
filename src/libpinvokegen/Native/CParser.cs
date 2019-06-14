namespace PinvokeGen.Native
{
    public class CParser
    {
        public CModel Parse()
        {
            var model = new CModel("libgit2");

            var gitError = model.AddTypedef(new CStructType(
                new CField(CType.Char.Pointer(), "message"),
                new CField(CType.Int.Value(), "klass")
            ), "git_error");

            var gitBuf = model.AddTypedef(new CStructType(), "git_buf");

            model.AddFunction(CType.Int.Value(), "git_libgit_init");
            model.AddFunction(CType.Int.Value(), "git_libgit_shutdown");
            model.AddFunction(CType.Void.Value(), "git_libgit2_version",
                new CParameter(CType.Int.Pointer(), "major"),
                new CParameter(CType.Int.Pointer(), "minor"),
                new CParameter(CType.Int.Pointer(), "rev")
            );
            model.AddFunction(CType.Int.Value(), "git_buf_contains_nul",
                new CParameter(gitBuf.Pointer(), "buf")
            );
            model.AddFunction(CType.Int.Value(), "git_buf_grow",
                new CParameter(gitBuf.Pointer(), "buffer"),
                new CParameter(CType.SizeT.Value(), "target_size")
            );
            model.AddFunction(CType.Int.Value(), "git_buf_set",
                new CParameter(gitBuf.Pointer(), "buffer"),
                new CParameter(CType.Void.ConstPointer(), "data"),
                new CParameter(CType.SizeT.Value(), "datalen")
            );
            model.AddFunction(gitError.ConstPointer(), "git_error_last");
            model.AddFunction(CType.Void.Value(), "git_error_set_str",
                new CParameter(CType.Int.Value(), "error_class"),
                new CParameter(CType.Char.ConstPointer(), "string")
            );

            return model;
        }
    }
}
