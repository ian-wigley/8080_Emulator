using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automation_work
{
    public class Output
    {
        public Output()
        {

        }

        public void Draw()
        {
        }


    }
}


/*
SuperStrict
Import "cpu.bmx"

Function FastGLDrawPixmap(p:TPixmap, x:Int, y:Int)
	SetBlend(SOLIDBLEND)
	glDisable(GL_TEXTURE_2D)
	glpixelzoom(1, -1)
	glRasterPos2i(0, 0)
	glBitmap(0, 0, 0, 0, x, -y, Null)
	glPixelStorei(GL_UNPACK_ROW_LENGTH, p.pitch Shr 2)
	glDrawPixels(p.width, p.height, GL_RGBA, GL_UNSIGNED_BYTE, p.pixels)
	SetBlend(ALPHABLEND)
End Function

Global video: Tvideo = New Tvideo

' Plain unrotated direct video output
Rem
Type Tvideo
	Field screen: TPixmap
	Field screenptr: Int Ptr
	Field pitch: Int
	Field x: Int
	Field y: Int
	
	Method Init()
		screen = CreatePixmap(256, 224, PF_RGBA8888)
		screen.ClearPixels(0)
		screenptr = Int Ptr(screen.pixels)
		pitch = screen.pitch Shr 2
		x = 20
		y = 40
		
		DebugLog("Video initialized.")
	End Method
	
	Method Draw()
		CopyScreen()
		SetColor(255, 255, 255)
		
		' Perform a fast OpenGL drawpixel instead of DrawPixmap because it's slow
		FastGLDrawPixmap(screen, x, y)
		
		SetColor(200, 200, 200)
		DrawLine(x, y-1, x+screen.width, y-1)
		DrawLine(x+screen.width, y, x+screen.width, y+screen.height)
		DrawLine(x, y+screen.height, x+screen.width, y+screen.height)
		DrawLine(x-1, y-1, x-1, y+screen.height)
	End Method
	
	Method CopyScreen()
		For Local j:Int=0 Until 224
			Local src:Int = $2400 + (j Shl 5)

			Local dst:Int = j*pitch
			For Local i:Int=0 Until 32
				Local vram:Byte = cpu.memoryptr[src]
				src :+ 1

				For Local b:Int=0 Until 8
					Local color:Int = 0
					If (vram&1) Then color = $FFFFFFFF
					screenptr[dst] = color
					dst :+ 1
					vram = vram Shr 1
				Next
			Next
		Next
	End Method
End Type
End Rem

' 2x scale video with color and scanline effect
Type Tvideo
	Field screen: TPixmap
	Field screenptr: Int Ptr
	Field pitch: Int
	Field x: Int
	Field y: Int
	Field color:Int[32,2]
	Field color2:Int[32,2]
	Field color3:Int[32*2]
	
	Method Init()
		screen = CreatePixmap(224*2, 256*2, PF_RGBA8888)

		screen.ClearPixels(0)
		screenptr = Int Ptr(screen.pixels)
		pitch = screen.pitch Shr 2
		x = 20
		y = 40
		
		For Local i:Int=0 Until 32
			color[i, 0] = 0 ' Black
			color2[i, 0] = EndianFlip($44444400) ' Black
			
			If(i>=26 And i<=27) Then
				color[i, 1] = EndianFlip($FF000000) ' Red
				color2[i, 1] = EndianFlip($AA000000) ' Red
			Else If(i>=2 And i<=7)
				color[i, 1] = EndianFlip($00FF0000) ' Green
				color2[i, 1] = EndianFlip($00AA0000) ' Green
			Else
				color[i, 1] = EndianFlip($FFFFFF00) ' White
				color2[i, 1] = EndianFlip($AAAAAA00) ' White
			End If
		Next
		
		DebugLog("Video initialized.")
	End Method
	
	Method Draw()
		CopyScreen()
		SetColor(255, 255, 255)

		' Perform a fast OpenGL drawpixel instead of DrawPixmap because it's slow
		FastGLDrawPixmap(screen, x, y)
		
		SetColor(200, 200, 200)
		DrawLine(x, y-1, x+screen.width, y-1)
		DrawLine(x+screen.width, y, x+screen.width, y+screen.height)
		DrawLine(x, y+screen.height, x+screen.width, y+screen.height)
		DrawLine(x-1, y-1, x-1, y+screen.height)
	End Method
	
	Method CopyScreen()		
		For Local j:Int=0 Until 224
			Local src:Int = $2400 + (j Shl 5)

			Local dst:Int = (screen.height-2)*pitch + j Shl 1
			For Local i:Int=0 Until 32
				Local vram:Byte = cpu.memoryptr[src]
				src :+ 1

				For Local bit:Int=0 Until 8
					screenptr[dst] = color[i, vram&1]
					screenptr[dst+1] = color2[i, vram&1]

					dst :- pitch Shl 1
					vram = vram Shr 1
				Next
			Next
		Next
	End Method
End Type

Rem
' Rotated 1x scale video output B&W
Type Tvideo
	Field screen: TPixmap
	Field screenptr: Int Ptr
	Field pitch: Int
	Field x: Int
	Field y: Int
	
	Method Init()
		screen = CreatePixmap(224, 256, PF_RGBA8888)
		screen.ClearPixels(0)
		screenptr = Int Ptr(screen.pixels)
		pitch = screen.pitch Shr 2
		x = 20
		y = 40
		
		DebugLog("Video initialized.")
	End Method
	
	Method Draw()
		CopyScreen()
		SetColor(255, 255, 255)
		
		' Perform a fast OpenGL drawpixel instead of DrawPixmap because it's slow
		FastGLDrawPixmap(screen, x, y)
		
		SetColor(200, 200, 200)
		DrawLine(x, y-1, x+screen.width, y-1)
		DrawLine(x+screen.width, y, x+screen.width, y+screen.height)
		DrawLine(x, y+screen.height, x+screen.width, y+screen.height)
		DrawLine(x-1, y-1, x-1, y+screen.height)
		
	End Method
	
	Method CopyScreen()
		For Local j:Int=0 Until 224
			Local src:Int = $2400 + (j Shl 5)

			Local dst:Int = 255*pitch + j
			For Local i:Int=0 Until 32
				Local vram:Byte = cpu.memoryptr[src]
				src :+ 1

				For Local b:Int=0 Until 8
					Local color:Int = 0
					If (vram&1) Then color = $FFFFFFFF
					screenptr[dst] = color
					dst :- pitch
					vram = vram Shr 1
				Next
			Next
		Next
	End Method
End Type
End Rem

*/
