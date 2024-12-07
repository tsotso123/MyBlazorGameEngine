let canvas = document.getElementById("gameCanvas");
let ctx = canvas.getContext("2d");
ctx.imageSmoothingEnabled = false;

let uiCanvas = document.getElementById("UiCanvas");
let uiCtx = uiCanvas.getContext("2d");
uiCtx.imageSmoothingEnabled = false;

const offscreenCanvas = document.createElement('canvas');
offscreenCanvas.width = canvas.width;
offscreenCanvas.height = canvas.height;
const offscreenContext = offscreenCanvas.getContext('2d');
offscreenContext.imageSmoothingEnabled = false;




//let imgUnitBg = new Image();
//imgUnitBg.src = "assets/Ui/unit icon bg.png";
//imgUnitBg.onload = () => {
//    uiCtx.drawImage(imgUnitBg, 50, 500, 64, 64);
//    console.log("rendered ui bg");
//};

const vertexShaderSource = `
attribute vec4 a_position;
attribute vec2 a_texCoord;
varying vec2 v_texCoord;
void main() {
    gl_Position = a_position;
    v_texCoord = a_texCoord;
}`;

const fragmentShaderSource = `
precision mediump float;
varying vec2 v_texCoord;
uniform sampler2D u_image;
void main() {
    gl_FragColor = texture2D(u_image, v_texCoord);
}`;
function initWebGL(canvasGL) {

    const gl = canvasGL.getContext("webgl", {
        alpha: true,
        premultipliedAlpha: true,
    });
    gl.enable(gl.BLEND);
    gl.blendFunc(gl.SRC_ALPHA, gl.ONE_MINUS_SRC_ALPHA);

    gl.viewport(0, 0, canvasGL.width, canvasGL.height);
    if (!gl) {
        console.error("WebGL initialization failed: ", canvas.getContext("webgl"));
        alert("Your browser does not support WebGL.");
        return null;
    }
    gl.clearColor(0, 0, 0, 0);
    gl.clear(gl.COLOR_BUFFER_BIT);

    return gl;
}

function createShader(gl, type, source) {
    const shader = gl.createShader(type);
    gl.shaderSource(shader, source);
    gl.compileShader(shader);
    if (!gl.getShaderParameter(shader, gl.COMPILE_STATUS)) {
        console.error(gl.getShaderInfoLog(shader));
        gl.deleteShader(shader);
        return null;
    }
    return shader;
}

function createProgram(gl, vertexShaderSource, fragmentShaderSource) {
    const vertexShader = createShader(gl, gl.VERTEX_SHADER, vertexShaderSource);
    const fragmentShader = createShader(gl, gl.FRAGMENT_SHADER, fragmentShaderSource);
    const program = gl.createProgram();
    gl.attachShader(program, vertexShader);
    gl.attachShader(program, fragmentShader);
    gl.linkProgram(program);
    if (!gl.getProgramParameter(program, gl.LINK_STATUS)) {
        console.error(gl.getProgramInfoLog(program));
        return null;
    }
    return program;
}
//function createImageVertices(imageWidth, imageHeight, canvasWidth, canvasHeight, posX, posY, flipHorizontally = false) {
//    // Calculate normalized width and height based on canvas dimensions
//    const x0 = (posX / canvasWidth) * 2 - 1;    // Left (normalized)
//    const y0 = (posY / canvasHeight) * 2 - 1;   // Top (normalized)
//    const x1 = ((posX + imageWidth) / canvasWidth) * 2 - 1;  // Right (normalized)
//    const y1 = ((posY + imageHeight) / canvasHeight) * 2 - 1; // Bottom (normalized)

//    // If flipHorizontally is true, swap the texture coordinates for horizontal flip
//    const s0 = flipHorizontally ? 1.0 : 0.0;
//    const s1 = flipHorizontally ? 0.0 : 1.0;

//    return new Float32Array([
//        // X, Y (position), Texture Coordinates (s, t)
//        x0, y1, s0, 1.0,  // Bottom-left
//        x1, y1, s1, 1.0,  // Bottom-right
//        x0, y0, s0, 0.0,  // Top-left
//        x1, y0, s1, 0.0   // Top-right
//    ]);
//}
function createImageVertices(imageWidth, imageHeight, canvasWidth, canvasHeight, posX, posY, flipHorizontally = false) {
    // Calculate normalized width and height based on canvas dimensions
    const x0 = (posX / canvasWidth) * 2 - 1;    // Left (normalized)
    const y0 = 1 - (posY / canvasHeight) * 2;   // Top (normalized)
    const x1 = ((posX + imageWidth) / canvasWidth) * 2 - 1;  // Right (normalized)
    const y1 = 1 - ((posY + imageHeight) / canvasHeight) * 2; // Bottom (normalized)

    // If flipHorizontally is true, swap the texture coordinates for horizontal flip
    const s0 = flipHorizontally ? 1.0 : 0.0;
    const s1 = flipHorizontally ? 0.0 : 1.0;

    return new Float32Array([
        // X, Y (position), Texture Coordinates (s, t)
        x0, y1, s0, 1.0,  // Bottom-left
        x1, y1, s1, 1.0,  // Bottom-right
        x0, y0, s0, 0.0,  // Top-left
        x1, y0, s1, 0.0   // Top-right
    ]);
}


// Usage
let cnv = document.getElementById("testCanvas");
let cnvCont = cnv.getContext("2d");
cnvCont.imageSmoothingEnabled = false;

//const gl = initWebGL(cnv);
//const program = createProgram(gl, vertexShaderSource, fragmentShaderSource);

//const imageGL = new Image();
//imageGL.src = "assets/demon.png"; // Image URL or source
//imageGL.onload = function () {
//    renderImage(gl, program, imageGL,0,0);
//};

function webGLRenderImage(x,y,imagePath,flipHorizontally=false) {
    //load image, from dic at imagePath
    img = imageCache["assets/" + imagePath];

    renderImage(gl, program, img, x,y,flipHorizontally);
}

function renderImage(gl, program, image, x,y,flipHorizontally = false) {
    // Set up vertices for a rectangle (two triangles)
    //const vertices = new Float32Array([
    //    -1.0, -1.0, 0.0, 1.0,
    //    1.0, -1.0, 1.0, 1.0,
    //    -1.0, 1.0, 0.0, 0.0,
    //    1.0, 1.0, 1.0, 0.0,
    //]);

    const vertices = createImageVertices(image.width, image.height, cnv.width, cnv.height, x, y, flipHorizontally);

    const buffer = gl.createBuffer();
    gl.bindBuffer(gl.ARRAY_BUFFER, buffer);
    gl.bufferData(gl.ARRAY_BUFFER, vertices, gl.STATIC_DRAW);

    // Look up locations in the program
    const positionLocation = gl.getAttribLocation(program, "a_position");
    const texCoordLocation = gl.getAttribLocation(program, "a_texCoord");

    // Enable attributes and set pointers
    gl.enableVertexAttribArray(positionLocation);
    gl.vertexAttribPointer(positionLocation, 2, gl.FLOAT, false, 16, 0);
    gl.enableVertexAttribArray(texCoordLocation);
    gl.vertexAttribPointer(texCoordLocation, 2, gl.FLOAT, false, 16, 8);

    // Load texture
    const texture = gl.createTexture();

    gl.bindTexture(gl.TEXTURE_2D, texture);

    // Flip the image's Y axis to match WebGL's texture coordinate space
    //gl.pixelStorei(gl.UNPACK_FLIP_Y_WEBGL, true);//
    

    // Load the image data into the texture
    gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, image);

    // Set texture parameters to avoid smoothing for pixel art
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.NEAREST);  // Minification
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.NEAREST);  // Magnification
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);

    //gl.bindTexture(gl.TEXTURE_2D, texture);
    //gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
    //gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
    //gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.LINEAR);
    //gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.LINEAR);

    //// Upload the image into the texture
    //gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, image);

    // Draw   
    //gl.clearColor(0, 0, 0, 0);  // 0.0 alpha ensures transparency
    //gl.clear(gl.COLOR_BUFFER_BIT);

    gl.useProgram(program);
    gl.drawArrays(gl.TRIANGLE_STRIP, 0, 4);
}




let keyStates = {};

function clearCanvas() {
    return new Promise((resolve) => {
        ctx.clearRect(0, 0, canvas.width, canvas.height);  // Clear canvas
        resolve();
    });
    

    //uiCtx.clearRect(0, canvas.height - canvas.height / 2, canvas.width, canvas.height / 2);
    
    //offscreenContext.clearRect(0, 0, canvas.width, canvas.height);

}

function webGlClear() {
    gl.clearColor(0, 0, 0, 0);  // 0.0 alpha ensures transparency
    gl.clear(gl.COLOR_BUFFER_BIT);
}


// Store all preloaded images in an object
var imageCache = {};

async function preloadImage(imagePath) {
    return new Promise((resolve, reject) => {
        let img = new Image();
        img.src = imagePath;

        img.onload = async () => {
            // Cache the image once it's loaded
            imageCache[imagePath] = img;
            
            //resolve(img); // Resolve the promise once the image is loaded

            try {
                // Fetch the image as ArrayBuffer (binary data)
                const response = await fetch(img.src);
                const arrayBuffer = await response.arrayBuffer();

                //const base64String = arrayBufferToBase64(arrayBuffer);

                const base64String = btoa(String.fromCharCode(...new Uint8Array(arrayBuffer)));
                resolve(base64String); // Resolve the promise with the image data (ArrayBuffer)
                console.log("preloaded");
            } catch (error) {
                reject("Failed to load image data: " + error);
            }

        };

        img.onerror = (error) => {
            reject("Failed to load image: " + imagePath);
        };
        
        
    });
}
// Helper function to convert ArrayBuffer to Base64 string
function arrayBufferToBase64(buffer) {
    let binary = '';
    const bytes = new Uint8Array(buffer);
    const length = bytes.byteLength;
    for (let i = 0; i < length; i++) {
        binary += String.fromCharCode(bytes[i]);
    }
    return window.btoa(binary); // Convert binary string to Base64
}

function drawOnUi(x, y, img, width,height,flipHorizontally = false) {
    uiCtx.save();
    if (flipHorizontally) {
        uiCtx.scale(-1, 1); // Flip the image horizontally
        uiCtx.drawImage(imageCache["assets/Ui/" + img], -x - width, y, width, height);
    }
    else {
        uiCtx.drawImage(imageCache["assets/Ui/" + img], x, y, width, height);
    }
    uiCtx.restore();
}

function finalDraw()
{
    ctx.drawImage(offscreenCanvas, 0, 0);  
}

function clearAt(x,y,width,height)
{
    ctx.clearRect(x, y, width, height);
    //offscreenContext.clearRect(x, y, width, height);
}

function drawWithWidthAndHeight(x, y, img, tmpWidth, tmpHeight,flipHorizontally = false) {
    //playerImage.src = 'assets/' + img;

    //ctx.save(); // Save the current context state
    //if (flipHorizontally) {
    //    ctx.scale(-1, 1); // Flip the image horizontally
    //    ctx.drawImage(imageCache["assets/" + img], -x-32, y, 32, 32);
    //}
    //else {
    //    ctx.drawImage(imageCache["assets/" + img], x, y, 32, 32);
    //}
    //ctx.restore(); // Restore the original state

    offscreenContext.save(); // Save the current context state
    tmpImg = imageCache["assets/" + img];
    if (flipHorizontally) {
        offscreenContext.scale(-1, 1); // Flip the image horizontally
        //offscreenContext.drawImage(imageCache["assets/" + img], -x - 32, y, 32, 32);      
        offscreenContext.drawImage(tmpImg, -x - tmpWidth, y, tmpWidth, tmpHeight);
    }
    else {
        //offscreenContext.drawImage(imageCache["assets/" + img], x, y, 32, 32);
        offscreenContext.drawImage(tmpImg, x, y, tmpWidth, tmpHeight);
    }
    offscreenContext.restore(); // Restore the original state


    //ctx.drawImage(imageCache["assets/" + img], x, y, 32, 32);


    //playerImage.onload = function () {
    //    ctx.drawImage(playerImage, x, y, 32, 32);  // Draw player
    //};

}



function drawAll(listOfUnits) {
    clearCanvas();
    //units = JSON.parse(listOfUnits);
    //units.forEach(unit => {

    //    draw(unit.X, unit.Y, unit.animationToDraw, unit.flipAnimation, unit.width, unit.height, unit.rotate)
    //});
    units = JSON.parse(listOfUnits);
    return new Promise((resolve) => {
        //clearCanvas();
        units.forEach(unit => {
            draw(unit.X, unit.Y, unit.animationToDraw, unit.flipAnimation, unit.width, unit.height, unit.rotate)
        });
        resolve();
    });

    
    
    //finalDraw();
}




//async function drawAll(listOfUnits) {
//    // Use a Promise to ensure all draws are awaited
//    const drawPromises = listOfUnits.map(async unit => {      
//        await draw(unit.X, unit.Y, unit.animationToDraw, unit.flipAnimation);
//    });
//    // Wait for all drawing promises to resolve
//    await Promise.all(drawPromises);
//}

function draw(x, y, img, flipHorizontally=false,width=0,height=0,rotate=0) {

    ctx.save(); // Save the current context state
    
    tmpImg = imageCache["assets/" + img];

    tmpHeight = tmpImg.height;
    tmpWidth = tmpImg.width;
    if (width != 0 && height != 0)
    {
        tmpHeight = height;
        tmpWidth = width;
    }

    if (rotate != 0)
    {
        // Translate to the center of the image
        ctx.translate(x + tmpWidth / 2, y + tmpHeight / 2);

        // Rotate the canvas by the desired angle (in radians)
        ctx.rotate(rotate);

        if (flipHorizontally) {
            ctx.scale(-1, 1); // Flip the image horizontally
        }
        // Draw the image, offset by half its width and height to keep it centered
        ctx.drawImage(tmpImg, -tmpWidth / 2, -tmpHeight / 2, tmpWidth, tmpHeight);
        ctx.restore();
        return;
    }

    if (flipHorizontally) {
        ctx.scale(-1, 1); // Flip the image horizontally

        ctx.drawImage(tmpImg, -x - tmpWidth, y, tmpWidth, tmpHeight);
    }
    else {

        ctx.drawImage(tmpImg, x, y, tmpWidth, tmpHeight);
    }
    ctx.restore(); // Restore the original state

    //console.log(tmpImg)
    
}


// Get the canvas position relative to the viewport
const canvasRect = canvas.getBoundingClientRect();

// Calculate the x and y offset
const offsetX = canvasRect.left;
const offsetY = canvasRect.top;
window.addEventListener('click', function (event) {
    // Get the position of the click
    const x = event.clientX - offsetX;
    const y = event.clientY;

    DotNet.invokeMethodAsync('game', 'HandleClick', x,y);
});

// Continuous key state handling
window.addEventListener("keydown", function (event) {
    if (!keyStates[event.key]) {
        keyStates[event.key] = true;
        DotNet.invokeMethodAsync('game', 'HandleKeyDown', event.key);
    }
});

window.addEventListener("keyup", function (event) {
    keyStates[event.key] = false;
    DotNet.invokeMethodAsync('game', 'HandleKeyUp', event.key);
});



