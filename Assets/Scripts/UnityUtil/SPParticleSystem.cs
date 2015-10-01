using UnityEngine;
using System.Collections.Generic;

public interface SPParticle {
	void i_update(Object context);
	bool should_remove(Object context);
	void do_remove(Object context);
}

public class SPParticleSystem {
	public static SPParticleSystem cons() {
		return (new SPParticleSystem()).i_cons();
	}

	private List<SPParticle> _particles = new List<SPParticle>(), _to_remove = new List<SPParticle>(), _to_add = new List<SPParticle>();
	private SPNode _anchor;

	private SPParticleSystem i_cons() {
		return this;
	}

	public void add_particle(SPParticle p) { _to_add.Add(p); }
	public void i_update(Object context) {
		for (int i = 0; i < _to_add.Count; i++) {
			SPParticle itr = _to_add[i];
			_particles.Add(itr);
		}
		_to_add.Clear();

		for (int i = 0; i < _particles.Count; i++) {
			SPParticle itr = _particles[i];
			itr.i_update(context);
			if (itr.should_remove(context)) {
				itr.do_remove(context);
				_to_remove.Add(itr);
			}
		}

		for (int i = 0; i < _to_remove.Count; i++) {
			SPParticle itr = _to_remove[i];
			_particles.Remove(itr);
		}
		_to_remove.Clear();
	}
	public void clear(Object context) {
		for (int i = 0; i < _particles.Count; i++) {
			SPParticle itr = _particles[i];
			itr.do_remove(context);
		}
		_particles.Clear();
	}
}