﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnderworldEditor
{
    class TileMapUI
    {
        public static void PopulateWorldNode(TreeNode node, int index, objects.ObjectInfo[] objList)
        {
            while (index != 0)
            {
                TreeNode newnode = node.Nodes.Add(index + ". " + objects.ObjectName(objList[index].item_id, main.curgame));
                newnode.Tag = index;
                if (objects.isContainer(objList[index].item_id))
                {
                    PlayerDatUI.PopulateLinkedInventoryNode(newnode, objList[index].link, objList);
                }
                index = objList[index].next;
                //else
                //{
                //    PopulateInventoryMagicLink(newnode, index, objList);
                //}
            }
        }

        public static void LoadTileMap(int blockno, main MAIN)
        {
            //MAIN.TreeTiles.Nodes.Clear();
            MAIN.tilemap = new TileMap();
            if (MAIN.uwblocks[blockno].DataLen < 0x7c06)
            {
                return;
            }
            MAIN.tilemap.InitTileMap(MAIN.uwblocks[blockno].Data, 0, blockno, MAIN.uwblocks[blockno].Address);
            if (main.curgame == 1)
            {
                MAIN.tilemap.BuildTextureMap(MAIN.uwblocks[blockno + 18].Data, ref MAIN.tilemap.ceilingtexture);
            }
            else
            {
                MAIN.tilemap.BuildTextureMap(MAIN.uwblocks[blockno + 80].Data, ref MAIN.tilemap.ceilingtexture);
            }
            //Temporarily output to treeview for testing.
           // MAIN.txtMap.Text = "";
            //for (int x = 0; x <= 63; x++)
            //{
               
            //   TreeNode xnode = MAIN.TreeTiles.Nodes.Add("X=" + x);
            //    for (int y = 0; y <= 63; y++)
            //    {
            //        TreeNode ynode = xnode.Nodes.Add("Y=" + y);
            //        ynode.Tag = x + "," + y;
            //        //MAIN.txtMap.Text += MAIN.tilemap.Tiles[x, y].tileType;
            //    }
            //    //MAIN.txtMap.Text += "\n";
            //}

            MAIN.worldObjects = new objects();
            MAIN.worldObjects.InitWorldObjectList(MAIN.uwblocks[blockno].Data, 64 * 64 * 4 , MAIN.uwblocks[blockno].Address);
            MAIN.TreeWorldObjects.Nodes.Clear();
            for (int i = 0; i <= MAIN.worldObjects.objList.GetUpperBound(0); i++)
            {
                TreeNode newnode = MAIN.TreeWorldObjects.Nodes.Add(i + ". " + objects.ObjectName(MAIN.worldObjects.objList[i].item_id, main.curgame));
                newnode.Tag = i;
            }

            MAIN.TreeWorldByTile.Nodes.Clear();
            for (int x = 0; x <= 63; x++)
            {
                for (int y = 0; y <= 63; y++)
                {
                    if (MAIN.tilemap.Tiles[x, y].indexObjectList != 0)
                    {
                        TreeNode xynode = MAIN.TreeWorldByTile.Nodes.Add(x + "," + y);
                        TileMapUI.PopulateWorldNode(xynode, MAIN.tilemap.Tiles[x, y].indexObjectList, MAIN.worldObjects.objList);
                    }
                }
            }
            
            MAIN.PicMap.Image = ArtUI.UWMap(MAIN.tilemap, MAIN.tex);
            MAIN.PicMap.Height = 64 * (int)MAIN.numMapZoom.Value;
            MAIN.PicMap.Width = 64 * (int)MAIN.numMapZoom.Value;

        }


        //public static void LoadTileInfoFromSelectedNode(main MAIN)
        //{
        //    if (MAIN.TreeTiles.SelectedNode.Tag != null)
        //    {
        //        //Load tile info for tagged value.
        //        string X = MAIN.TreeTiles.SelectedNode.Tag.ToString().Split(',')[0];
        //        string Y = MAIN.TreeTiles.SelectedNode.Tag.ToString().Split(',')[1];
        //        int x; int y;
        //        if (int.TryParse(X, out x))
        //        {
        //            if (int.TryParse(Y, out y))
        //            {
        //                LoadInfoForTileXY(MAIN, x, y);
        //            }
        //        }
        //    }
        //}

        public static void LoadInfoForTileXY(main MAIN, int x, int y)
        {
            main.isLoading = true;
            MAIN.curTileX = x;
            MAIN.curTileY = y;
            MAIN.lblCurrentTile.Text = "Current Tile " + x + "," + y;// + " " + MAIN.TreeTiles.SelectedNode.Tag.ToString();
            MAIN.CmbTileType.Text = TileMap.GetTileTypeText(MAIN.tilemap.Tiles[x, y].tileType);
            MAIN.NumFloorHeight.Value = MAIN.tilemap.Tiles[x, y].floorHeight;
            MAIN.NumFloorTexture.Value = MAIN.tilemap.Tiles[x, y].floorTexture;
            MAIN.NumWallTexture.Value = MAIN.tilemap.Tiles[x, y].wallTexture;
            MAIN.NumIndexObjectList.Value = MAIN.tilemap.Tiles[x, y].indexObjectList;
            int actualtexture = MAIN.tilemap.GetMappedFloorTexture(TileMap.fSELF, MAIN.tilemap.Tiles[x, y]);
            MAIN.LblMappedFloorTexture.Text = actualtexture.ToString()
                + " " + MAIN.UWGameStrings.GetTextureName(actualtexture, main.curgame);
            MAIN.NumTileFlags.Value = MAIN.tilemap.Tiles[x, y].flags;
            MAIN.NumDoorBit.Value = MAIN.tilemap.Tiles[x, y].doorBit;
            MAIN.NumNoMagic.Value = MAIN.tilemap.Tiles[x, y].noMagic;
            main.isLoading = false;
        }

        public static void PopulateWorldObjects(int index, main MAIN)
        {
            main.isLoading = true;
            objects.ObjectInfo obj = MAIN.worldObjects.objList[index];
            MAIN.CurWorldObject = index;
            MAIN.PopulateObjectUI(obj,
                MAIN.CmbWorldItem_ID, MAIN.ChkWorldEnchanted,
                MAIN.ChkWorldIsQuant, MAIN.ChkWorldDoorDir,
                MAIN.ChkWorldInvis, MAIN.NumWorldXPos,
                MAIN.NumWorldYPos, MAIN.NumWorldZpos,
                MAIN.NumWorldHeading, MAIN.NumWorldFlags,
                MAIN.NumWorldQuality, MAIN.NumWorldOwner,
                MAIN.NumWorldNext, MAIN.NumWorldLink);
            main.isLoading = false;
        }

        public static void ApplyTileChanges(int x, int y, main MAIN)
        {
            

            MAIN.tilemap.Tiles[x, y].tileType = (short)TileMap.GetTileTypeInt(MAIN.CmbTileType.Text);
            MAIN.tilemap.Tiles[x, y].floorHeight = (short)MAIN.NumFloorHeight.Value;
            MAIN.tilemap.Tiles[x, y].flags = (short)MAIN.NumTileFlags.Value;
            MAIN.tilemap.Tiles[x, y].floorTexture = (short)MAIN.NumFloorTexture.Value;
            MAIN.tilemap.Tiles[x, y].noMagic = (short)MAIN.NumNoMagic.Value;
            MAIN.tilemap.Tiles[x, y].doorBit = (short)MAIN.NumDoorBit.Value;
            MAIN.tilemap.Tiles[x, y].indexObjectList = (short)MAIN.NumIndexObjectList.Value;
            MAIN.tilemap.Tiles[x, y].wallTexture = (short)MAIN.NumWallTexture.Value;

            int actualtexture = MAIN.tilemap.GetMappedFloorTexture(TileMap.fSELF, MAIN.tilemap.Tiles[x, y]);
            MAIN.LblMappedFloorTexture.Text = actualtexture.ToString()
                + " " + MAIN.UWGameStrings.GetTextureName(actualtexture, main.curgame);


            TileMap.TileInfo t = MAIN.tilemap.Tiles[x, y];

            //Shift the bits to construct my data
            int tileType = t.tileType;
            int floorHeight = (t.floorHeight / 2) << 4;

            int ByteToWrite = tileType | floorHeight;//| floorTexture | noMagic;//This will be set in the original data
            MAIN.levarkbuffer[t.FileAddress] = (char)(ByteToWrite);
            int flags = t.flags & 0x3;
            int floorTexture = t.floorTexture << 2;
            int noMagic = t.noMagic << 6;
            int DoorBit = t.doorBit << 7;
            ByteToWrite = floorTexture | noMagic | DoorBit | flags;
            MAIN.levarkbuffer[t.FileAddress + 1] = (char)(ByteToWrite);

            ByteToWrite = ((t.indexObjectList & 0x3FF) << 6) | (t.wallTexture & 0x3F);
            MAIN.levarkbuffer[t.FileAddress + 2] = (char)(ByteToWrite & 0xFF);
            MAIN.levarkbuffer[t.FileAddress + 3] = (char)((ByteToWrite >> 8) & 0xFF);

            MAIN.tilemap.Cleanup();
            MAIN.PicMap.Image = ArtUI.UWMap(MAIN.tilemap, MAIN.tex);
            MAIN.PicMap.Height = 64 * (int)MAIN.numMapZoom.Value;
            MAIN.PicMap.Width = 64 * (int)MAIN.numMapZoom.Value;


        }

    }
}
