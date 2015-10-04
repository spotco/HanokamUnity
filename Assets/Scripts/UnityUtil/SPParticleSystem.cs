using UnityEngine;
using System.Collections.Generic;

public interface SPParticle {
	void i_update(Object context);
	bool should_remove(Object context);
	void do_remove(Object context);
}
public abstract class SPGameEngineParticle : SPParticle, SPGameUpdateable, SPGameHierarchyElement {
	public void i_update(Object context) { this.i_update(context as GameEngineScene); }
	public bool should_remove(Object context) { return this.should_remove(context as GameEngineScene); }
	public void do_remove(Object context) { this.do_remove(context as GameEngineScene); }
	
	public virtual void i_update(GameEngineScene g) { throw new System.Exception(this.GetType()+" must Implement i_update(g)"); }
	public virtual bool should_remove(GameEngineScene g) { throw new System.Exception(this.GetType()+" must Implement should_remove(g)"); }
	public virtual void do_remove(GameEngineScene g) { throw new System.Exception(this.GetType()+" must Implement do_remove(g)"); }
	public virtual void add_to_parent(SPNode parent) { throw new System.Exception(this.GetType()+" must Implement add_to_parent(g)"); }
}

public class SPParticleSystem<T> where T : SPParticle {
	public static SPParticleSystem<T> cons() {
		return (new SPParticleSystem<T>()).i_cons();
	}

	private List<T> _particles = new List<T>(), _to_remove = new List<T>(), _to_add = new List<T>();
	private SPNode _anchor;

	private SPParticleSystem<T> i_cons() {
		return this;
	}

	public virtual void add_particle(T p) { _to_add.Add(p); }
	public virtual void i_update(Object context) {
		for (int i = 0; i < _to_add.Count; i++) {
			T itr = _to_add[i];
			_particles.Add(itr);
		}
		_to_add.Clear();

		for (int i = 0; i < _particles.Count; i++) {
			T itr = _particles[i];
			itr.i_update(context);
			if (itr.should_remove(context)) {
				itr.do_remove(context);
				_to_remove.Add(itr);
			}
		}

		for (int i = 0; i < _to_remove.Count; i++) {
			T itr = _to_remove[i];
			_particles.Remove(itr);
		}
		_to_remove.Clear();
	}
	public virtual void clear(Object context) {
		for (int i = 0; i < _particles.Count; i++) {
			T itr = _particles[i];
			itr.do_remove(context);
		}
		_particles.Clear();
	}
}