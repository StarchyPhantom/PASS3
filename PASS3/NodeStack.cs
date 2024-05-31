using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PASS3
{
	class NodeStack
	{
		//list and size
		List<TileNode> nodes;
		int size;

		/// <summary>
		/// stack to hold nodes
		/// </summary>
		public NodeStack ()
		{
			//set list and size
			nodes = new List<TileNode>();
			size = 0;
		}

		//PRE: node to add
		//POST: 
		//DESC: add node
		public void Push(TileNode node)
		{
			//add node, increase size
			nodes.Add(node);
			size++;
		}

		//PRE: 
		//POST: return node
		//DESC: remove and return node
		public TileNode Pop()
		{
			//result is guilty till proven innocent
			TileNode result = null;

			//if not empty, suspect is innocent
			if (!IsEmpty())
			{
				//set innocence
				result = Top();
				nodes.RemoveAt(size - 1);

				//reduce size
				size--;
			}

			//let the judge decide
			return result;
		}

		//PRE: 
		//POST: get the top tile node of the stack
		//DESC: 
		public TileNode Top()
		{
			//tilenode is null first
			TileNode result = null;

			//if not empty, tile node is the top
			if (!IsEmpty())
			{
				result = nodes[size - 1];
			}

			//return the node
			return result;
		}

		//PRE: 
		//POST: return int size
		//DESC: 
		public int Size()
		{
			//return size
			return size;
		}

		//PRE: 
		//POST: return if it is empty
		//DESC: 
		public bool IsEmpty()
		{
			//if not empty, return false
			if (size != 0)
			{
				return false;
			}

			//if empty, return true
			return true;
		}
	}
}
