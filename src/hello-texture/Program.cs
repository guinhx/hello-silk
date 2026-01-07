using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;

using System.Drawing;

var options = WindowOptions.Default with
{
    Size = new Vector2D<int>(800, 600),
    Title = "Hello Quad"
};

var window = Window.Create(options);
GL gl = default;

// buffers
uint vao = 0;
uint vbo = 0;
uint ebo = 0;

// shader program
uint program = 0;

window.Load += () => {
    gl = window.CreateOpenGL();

    vao = gl.GenVertexArray();
    gl.BindVertexArray(vao);

    var vertices = new float[] {
        //       aPosition     | aTexCoords
        0.5f,  0.5f, 0.0f,  1.0f, 1.0f,
        0.5f, -0.5f, 0.0f,  1.0f, 0.0f,
        -0.5f, -0.5f, 0.0f,  0.0f, 0.0f,
        -0.5f,  0.5f, 0.0f,  0.0f, 1.0f
    };

    vbo = gl.GenBuffer();
    gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);

    unsafe {
        fixed (float* buf = vertices)
            gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint) (vertices.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw);
    }

    var indices = new uint[]
    {
        0u, 1u, 3u,
        1u, 2u, 3u
    };

    ebo = gl.GenBuffer();
    gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo);

    unsafe {
        fixed (uint* buf = indices)
            gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint) (indices.Length * sizeof(uint)), buf, BufferUsageARB.StaticDraw);
    }

    const string vertexCode = @"
    #version 330 core

    layout (location = 0) in vec3 aPosition;
    // Add a new input attribute for the texture coordinates
    layout (location = 1) in vec2 aTextureCoord;

    // Add an output variable to pass the texture coordinate to the fragment shader
    // This variable stores the data that we want to be received by the fragment
    out vec2 frag_texCoords;

    void main()
    {
        gl_Position = vec4(aPosition, 1.0);
        // Assigin the texture coordinates without any modification to be recived in the fragment
        frag_texCoords = aTextureCoord;
    }";

    const string fragmentCode = @"
    #version 330 core

    // Receive the input from the vertex shader in an attribute
    in vec2 frag_texCoords;

    out vec4 out_color;

    void main()
    {
        // This will allow us to see the texture coordinates in action!
        out_color = vec4(frag_texCoords.x, frag_texCoords.y, 0.0, 1.0);
    }";

    uint vertexShader = gl.CreateShader(ShaderType.VertexShader);
    gl.ShaderSource(vertexShader, vertexCode);

    gl.CompileShader(vertexShader);

    gl.GetShader(vertexShader, ShaderParameterName.CompileStatus, out int vStatus);
    if (vStatus != (int) GLEnum.True)
        throw new Exception("Vertex shader failed to compile: " + gl.GetShaderInfoLog(vertexShader));

    uint fragmentShader = gl.CreateShader(ShaderType.FragmentShader);
    gl.ShaderSource(fragmentShader, fragmentCode);

    gl.CompileShader(fragmentShader);

    gl.GetShader(fragmentShader, ShaderParameterName.CompileStatus, out int fStatus);
    if (fStatus != (int) GLEnum.True)
        throw new Exception("Fragment shader failed to compile: " + gl.GetShaderInfoLog(fragmentShader));

    program = gl.CreateProgram();
    gl.AttachShader(program, vertexShader);
    gl.AttachShader(program, fragmentShader);
    gl.LinkProgram(program);

    gl.GetProgram(program, ProgramPropertyARB.LinkStatus, out int lStatus);
    if (lStatus != (int) GLEnum.True)
        throw new Exception("Program failed to link: " + gl.GetProgramInfoLog(program));

    gl.DetachShader(program, vertexShader);
    gl.DetachShader(program, fragmentShader);
    gl.DeleteShader(vertexShader);
    gl.DeleteShader(fragmentShader);

    unsafe {
        const uint positionLoc = 0;
        gl.EnableVertexAttribArray(positionLoc);
        gl.VertexAttribPointer(positionLoc, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*) 0);

        const uint texCoordLoc = 1;
        gl.EnableVertexAttribArray(texCoordLoc);
        gl.VertexAttribPointer(texCoordLoc, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)(3 * sizeof(float)));
    }

    gl.BindVertexArray(0);
    gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
    gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);

    gl.ClearColor(Color.CornflowerBlue);
};

window.Render += (double deltaTime) => {
    if (vao == 0 || program == 0)
        return;

    gl.Clear(ClearBufferMask.ColorBufferBit);

    unsafe {
        gl.BindVertexArray(vao);
        gl.UseProgram(program);
        gl.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, (void*) 0);
    }
};

window.Run();
